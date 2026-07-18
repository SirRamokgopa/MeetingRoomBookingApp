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
            ValidateBooking(bookingEntity);

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

            ValidateBooking(bookingFromDb);

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
