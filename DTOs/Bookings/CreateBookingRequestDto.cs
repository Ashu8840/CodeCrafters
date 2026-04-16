namespace Hotel_Booking_API.DTOs.Bookings;

public class CreateBookingRequestDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
