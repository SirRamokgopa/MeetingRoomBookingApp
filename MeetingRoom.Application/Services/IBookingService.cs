using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Services
{
    public interface IBookingService
    {
        Task<BookingDTO> CreateBooking(CreateBookingDTO booking);
        Task<BookingDTO> UpdateBooking(UpdateBookingDTO booking);
        Task DeleteBookingAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetBookings();
        Task<BookingDTO?> GetBookingById(int bookingId);
        Task<IEnumerable<RoomDTO>> GetRooms();
    }
}
