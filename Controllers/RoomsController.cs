using Hotel_Booking_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(HotelBookingDbContext dbContext) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchRooms(
        [FromQuery] string? location,
        [FromQuery] DateTime? checkInDate,
        [FromQuery] DateTime? checkOutDate,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? capacity,
        [FromQuery] List<int>? facilityIds)
    {
        if (checkInDate.HasValue && checkOutDate.HasValue && checkOutDate.Value.Date <= checkInDate.Value.Date)
        {
            return BadRequest("Check-out date must be after check-in date.");
        }

        var query = dbContext.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Facilities)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(r => r.Hotel.Location != null && r.Hotel.Location.Contains(location));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight <= maxPrice.Value);
        }

        if (capacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= capacity.Value);
        }

        if (facilityIds is { Count: > 0 })
        {
            foreach (var facilityId in facilityIds)
            {
                query = query.Where(r => r.Facilities.Any(f => f.Id == facilityId));
            }
        }

        if (checkInDate.HasValue && checkOutDate.HasValue)
        {
            var checkIn = checkInDate.Value.Date;
            var checkOut = checkOutDate.Value.Date;

            query = query.Where(r => !r.Bookings.Any(b =>
                b.Status == "Confirmed" &&
                checkIn < b.CheckOutDate &&
                checkOut > b.CheckInDate));
        }

        var rooms = await query
            .Select(r => new
            {
                r.Id,
                r.HotelId,
                HotelName = r.Hotel.Name,
                HotelLocation = r.Hotel.Location,
                r.RoomNumber,
                r.Capacity,
                r.PricePerNight,
                IsAvailable = r.IsAvailable ?? true,
                Facilities = r.Facilities.Select(f => f.Name).ToList()
            })
            .ToListAsync();

        return Ok(rooms);
    }
}
