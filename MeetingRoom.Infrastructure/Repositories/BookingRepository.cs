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
            _logger.LogInformation("Adding booking for room {RoomId} from {StartTime} to {EndTime}", newBooking.RoomId, newBooking.StartTime, newBooking.EndTime);

            try
            {
                var entity = newBooking.ToEntity();
                _context.Bookings.Add(entity);
                await _context.SaveChangesAsync();
                newBooking.Id = entity.Id;

                _logger.LogInformation("Booking {BookingId} added successfully", entity.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to add booking for room {RoomId}", newBooking.RoomId);
                throw new InvalidOperationException("Unable to create the booking due to a persistence issue.", ex);
            }
        }

        public async Task DeleteAsync(Booking booking)
        {
            _logger.LogInformation("Deleting booking {BookingId}", booking.Id);

            try
            {
                var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Delete requested for missing booking {BookingId}", booking.Id);
                    return;
                }

                _context.Bookings.Remove(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Booking {BookingId} deleted successfully", booking.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete booking {BookingId}", booking.Id);
                throw new InvalidOperationException("Unable to delete the booking due to a persistence issue.", ex);
            }
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all bookings");
                var entities = await _context.Bookings.ToListAsync();
                var bookings = entities.Select(e => e.ToDomain()).ToList();
                _logger.LogInformation("Retrieved {BookingCount} bookings", bookings.Count);
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve bookings");
                throw new InvalidOperationException("Unable to retrieve bookings.", ex);
            }
        }

        public async Task<Booking?> GetBookingAsync(int id)
        {
            _logger.LogInformation("Retrieving booking {BookingId}", id);

            try
            {
                var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
                if (entity == null)
                {
                    _logger.LogWarning("Booking {BookingId} was not found", id);
                }

                return entity?.ToDomain();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve booking {BookingId}", id);
                throw new InvalidOperationException("Unable to retrieve the booking.", ex);
            }
        }

        public async Task<Booking?> GetConflictingBookingAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            try
            {
                var entity = await _context.Bookings.FirstOrDefaultAsync(b =>
                    b.RoomId == roomId &&
                    (excludeBookingId == null || b.Id != excludeBookingId) &&
                    b.StartTime < endTime &&
                    b.EndTime > startTime);

                return entity?.ToDomain();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check for conflicting bookings for room {RoomId}", roomId);
                throw new InvalidOperationException("Unable to check for conflicting bookings.", ex);
            }
        }

        public async Task UpdateAsync(Booking booking)
        {
            _logger.LogInformation("Updating booking {BookingId} for room {RoomId}", booking.Id, booking.RoomId);

            try
            {
                var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Booking {BookingId} was not found for update", booking.Id);
                    return;
                }

                entity.UpdateFromDomain(booking);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Booking {BookingId} updated successfully", booking.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update booking {BookingId}", booking.Id);
                throw new InvalidOperationException("Unable to update the booking due to a persistence issue.", ex);
            }
        }
    }
}
