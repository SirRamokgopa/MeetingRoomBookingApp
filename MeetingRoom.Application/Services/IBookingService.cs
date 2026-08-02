using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Entities;

namespace MeetingRoom.Application.Services
{
    public interface IBookingService
    {
        Task<BookingDTO> CreateBooking(CreateBookingDTO booking);
        Task<BookingDTO> UpdateBooking(UpdateBookingDTO booking);
        Task DeleteBookingAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetBookings();
        Task<BookingDTO?> GetBookingById(int bookingId);
        Task ValidateBooking(Booking booking);
        Task<IEnumerable<RoomDTO>> GetRooms();
    }
}
