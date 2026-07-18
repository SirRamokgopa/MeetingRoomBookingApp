using MeetingRoom.API.Persistance;

namespace MeetingRoom.API.Services
{
    public interface IBookingService
    {
        void CreateBooking();
        Booking UpdateBooking(int bookingId);
        void DeleteBooking(int bookingId);
        IEnumerable<Booking> GetBookings();
        Booking GetBookingById(int bookingId);
        void ValidateBooking();
        IEnumerable<Room> GetRooms();
    }
}
