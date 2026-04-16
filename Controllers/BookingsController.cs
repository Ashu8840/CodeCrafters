using System.Security.Claims;
using Hotel_Booking_API.DTOs.Bookings;
using Hotel_Booking_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController(HotelBookingDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequestDto request)
    {
        if (request.CheckOutDate.Date <= request.CheckInDate.Date)
        {
            return BadRequest("Check-out date must be after check-in date.");
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user token.");
        }

        var room = await dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room is null)
        {
            return NotFound("Room not found.");
        }

        var checkIn = request.CheckInDate.Date;
        var checkOut = request.CheckOutDate.Date;

        var hasConflict = await dbContext.Bookings.AnyAsync(b =>
            b.RoomId == request.RoomId &&
            b.Status == "Confirmed" &&
            checkIn < b.CheckOutDate &&
            checkOut > b.CheckInDate);

        if (hasConflict)
        {
            return BadRequest("Room is not available for the selected dates.");
        }

        var nights = (checkOut - checkIn).Days;
        var totalPrice = nights * room.PricePerNight;

        var booking = new Booking
        {
            UserId = userId,
            RoomId = request.RoomId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            TotalPrice = totalPrice,
            Status = "Confirmed"
        };

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();

        await UpdateRoomCurrentAvailability(room.Id);

        return Ok(new
        {
            booking.Id,
            booking.UserId,
            booking.RoomId,
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.TotalPrice,
            booking.Status
        });
    }

    private async Task UpdateRoomCurrentAvailability(int roomId)
    {
        var room = await dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room is null)
        {
            return;
        }

        var today = DateTime.UtcNow.Date;
        var isCurrentlyBooked = await dbContext.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status == "Confirmed" &&
            today >= b.CheckInDate &&
            today < b.CheckOutDate);

        room.IsAvailable = !isCurrentlyBooked;
        await dbContext.SaveChangesAsync();
    }
}
