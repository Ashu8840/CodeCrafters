using Hotel_Booking_API.Models;

namespace Hotel_Booking_API.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
