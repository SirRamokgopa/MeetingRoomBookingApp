using MeetingRoom.API.DTOs;
using MeetingRoom.API.Persistance;
using MeetingRoom.API.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly MeetingRoomContext _context;

        public BookingService(MeetingRoomContext context)
        {
            _context = context;
        }

        public async Task<BookingDTO> CreateBooking(CreateBookingDTO booking)
        {
            var bookingEntity = booking.ToEntity();
            await ValidateBooking(bookingEntity);

            _context.Add(bookingEntity);
            await _context.SaveChangesAsync();

            return bookingEntity.ToDto();
        }

        public async Task<BookingDTO> UpdateBooking(UpdateBookingDTO booking)
        {

            var bookingFromDb = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (bookingFromDb == null)
            {
                throw new ArgumentException($"Booking with id {booking} not found");
            }

            bookingFromDb.UpdateFromDto(booking);

            await ValidateBooking(bookingFromDb);

            await _context.SaveChangesAsync();
            return bookingFromDb.ToDto();
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            // If booking is not found do nada (Not sure if this is best yet)
        }

        public async Task<IEnumerable<BookingDTO>> GetBookings()
        {
            var bookings = await _context.Bookings.Select(b => b.ToDto()).ToListAsync();
            return bookings;
        }

        public async Task<BookingDTO?> GetBookingById(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

            return booking?.ToDto();
        }

        public async Task<IEnumerable<RoomDTO>> GetRooms()
        {
            return await _context.Rooms
                .Select(r => new RoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity
                })
                .ToListAsync();
        }

        public async Task ValidateBooking(Booking booking)
        {
            // Check that room for booking exists
            var room = _context.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);
            if (room == null)
            {
                throw new ArgumentException($"Room with id {booking.RoomId} does not exist");
            };

            //Check that booking has title
            if (booking.Title == null || booking.Title.Trim() == "" || string.IsNullOrEmpty(booking.Title))
            {
                throw new ArgumentException("Booking title cannot be empty");
            }

            // Check that booking start time is before end time
            if (booking.StartTime >= booking.EndTime)
            {
                throw new ArgumentException("Booking start time must be before end time");
            }

            // Create new conflict booking exception with details of the conflicting booking
            var conflictingBooking = await GetConflictingBooking(booking);
            if (conflictingBooking is not null)
            {
                throw new BookingConflictException(conflictingBooking);
            }
        }

        private async Task<Booking?> GetConflictingBooking(Booking booking)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b =>
                b.RoomId == booking.RoomId &&
                b.Id != booking.Id &&
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime);
        }
    }

    public class BookingConflictException : Exception
    {
        public int ConflictingBookingId { get; }
        public int RoomId { get; }
        public DateTime ConflictingStartTime { get; }
        public DateTime ConflictingEndTime { get; }

        public BookingConflictException(Booking conflictingBooking)
            : base($"The requested time slot conflicts with an existing booking (Id: {conflictingBooking.Id}) " +
                   $"in room {conflictingBooking.RoomId} from {conflictingBooking.StartTime} to {conflictingBooking.EndTime}.")
        {
            ConflictingBookingId = conflictingBooking.Id;
            RoomId = conflictingBooking.RoomId;
            ConflictingStartTime = conflictingBooking.StartTime;
            ConflictingEndTime = conflictingBooking.EndTime;
        }
    }
}
