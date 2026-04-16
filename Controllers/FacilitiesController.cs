using Hotel_Booking_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilitiesController(HotelBookingDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFacilities()
    {
        var facilities = await dbContext.Facilities
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();

        return Ok(facilities);
    }
}
