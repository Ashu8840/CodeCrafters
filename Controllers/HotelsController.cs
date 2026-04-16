using Hotel_Booking_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(HotelBookingDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHotels([FromQuery] string? location)
    {
        var query = dbContext.Hotels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(x => x.Location != null && x.Location.Contains(location));
        }

        var hotels = await query
            .Select(h => new
            {
                h.Id,
                h.Name,
                h.Location,
                h.Description,
                RoomCount = h.Rooms.Count
            })
            .ToListAsync();

        return Ok(hotels);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHotelById(int id)
    {
        var hotel = await dbContext.Hotels
            .Include(h => h.Rooms)
            .ThenInclude(r => r.Facilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel is null)
        {
            return NotFound("Hotel not found.");
        }

        return Ok(new
        {
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.Description,
            Rooms = hotel.Rooms.Select(r => new
            {
                r.Id,
                r.RoomNumber,
                r.Capacity,
                r.PricePerNight,
                IsAvailable = r.IsAvailable ?? true,
                Facilities = r.Facilities.Select(f => f.Name).ToList()
            })
        });
    }
}
