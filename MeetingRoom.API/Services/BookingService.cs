using MeetingRoom.API.Persistance;

namespace MeetingRoom.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly MeetingRoomContext _context;

        public BookingService(MeetingRoomContext context)
        {
            _context = context;
        }

        public void CreateBooking(Booking booking)
        {
            // This method will be implemented to create a booking
            // and validate it before saving
            throw new NotImplementedException();
        }

        public Booking UpdateBooking(Booking booking)
        {
            var bookingFromDb = _context.Bookings.FirstOrDefault(b => b.Id == booking.Id);
            
            if (bookingFromDb == null)
            {
                throw new ArgumentException($"Booking with id {booking} not found");
            }

            ValidateBooking(booking);

            _context.SaveChanges();
            return bookingFromDb;
        }

        public void DeleteBooking(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            // If booking is not found do nada (Not sure if this is best yet)
        }

        public IEnumerable<Booking> GetBookings()
        {
            return _context.Bookings.ToList();
        }

        public Booking GetBookingById(int bookingId)
        {
            return _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
        }


        public IEnumerable<Room> GetRooms()
        {
            return _context.Rooms.ToList();
        }

        public void ValidateBooking(Booking booking)
        {
            // Validation logic will be implemented here
            throw new NotImplementedException();
        }

        private bool HasBookingConflict(Booking booking)
        {
            return _context.Bookings.Any(b =>
                b.RoomId == booking.RoomId &&
                b.Id != booking.Id && 
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime);
        }
    }
}
