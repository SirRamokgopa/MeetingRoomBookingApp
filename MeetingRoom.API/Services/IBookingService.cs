using MeetingRoom.API.Persistance;

namespace MeetingRoom.API.Services
{
    public interface IBookingService
    {
        void CreateBooking(Booking booking);
        Booking UpdateBooking(Booking booking);
        void DeleteBooking(int bookingId);
        IEnumerable<Booking> GetBookings();
        Booking GetBookingById(int bookingId);
        void ValidateBooking(Booking booking);
        IEnumerable<Room> GetRooms();
    }
}
