using MeetingRoom.API.DTOs;
using MeetingRoom.API.Persistance;

namespace MeetingRoom.API.Services
{
    public interface IBookingService
    {
        Task<BookingDTO> CreateBooking(CreateBookingDTO booking);
        Task<BookingDTO> UpdateBooking(UpdateBookingDTO booking);
        Task DeleteBookingAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetBookings();
        Task<BookingDTO?> GetBookingById(int bookingId);
        void ValidateBooking(Booking booking);
        Task<IEnumerable<RoomDTO>> GetRooms();
    }
}
