using Hotel_Booking_API.DTOs.Auth;
using Hotel_Booking_API.Models;
using Hotel_Booking_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(HotelBookingDbContext dbContext, IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Name, email, and password are required.");
        }

        var email = request.Email.Trim().ToLower();
        var existingUser = await dbContext.Users.AnyAsync(x => x.Email == email);
        if (existingUser)
        {
            return BadRequest("Email is already registered.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = PasswordHasher.HashPassword(request.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var token = jwtTokenService.GenerateToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token,
            Name = user.Name,
            Email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var email = request.Email.Trim().ToLower();
        var passwordHash = PasswordHasher.HashPassword(request.Password);

        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == passwordHash);

        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = jwtTokenService.GenerateToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token,
            Name = user.Name,
            Email = user.Email
        });
    }
}
