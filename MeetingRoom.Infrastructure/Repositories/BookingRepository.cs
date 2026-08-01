using MeetingRoom.Application.Abstractions.Repositories;
using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Mappings;
using MeetingRoom.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeetingRoom.Infrastructure.Repositories
{
    public class BookingRepository(MeetingRoomContext context, ILogger<BookingRepository> logger) : IBookingRepository
    {
        private readonly MeetingRoomContext _context = context;
        private readonly ILogger<BookingRepository> _logger = logger;

        public BookingRepository(MeetingRoomContext context) : this(context, NullLogger<BookingRepository>.Instance)
        {
        }

        public async Task AddAsync(Booking newBooking)
        {
            var entity = newBooking.ToEntity();
            _context.Bookings.Add(entity);
            await _context.SaveChangesAsync();
            newBooking.Id = entity.Id;
        }

        public async Task DeleteAsync(Booking booking)
        {
            var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
            if (entity == null)
            {
                return;
            }

            _context.Bookings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetAllBookingAsync()
        {
            var entities = await _context.Bookings.ToListAsync();
            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<Booking?> GetBookingAsync(int id)
        {
            var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            return entity?.ToDomain();
        }

        public async Task<Booking?> GetConflictingBookingAsucn(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var entity = await _context.Bookings.FirstOrDefaultAsync(b =>
                b.RoomId == roomId &&
                (excludeBookingId == null || b.Id != excludeBookingId) &&
                b.StartTime < endTime &&
                b.EndTime > startTime);

            return entity?.ToDomain();
        }

        public async Task UpdateAsync(Booking booking)
        {
            var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
            if (entity == null)
            {
                return;
            }

            entity.UpdateFromDomain(booking);
            await _context.SaveChangesAsync();
        }
    }
}
