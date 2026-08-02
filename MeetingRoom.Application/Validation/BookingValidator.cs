using MeetingRoom.Application.Abstractions.Repositories;
using MeetingRoom.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeetingRoom.Application.Validation
{
    /// <summary>
    /// Default <see cref="IBookingValidator"/> implementation. Collects every failing rule instead of
    /// throwing on the first one, and reports the outcome via <see cref="BookingValidationResult"/>.
    /// </summary>
    public class BookingValidator(IBookingRepository bookingRepository, IRoomRepository roomRepository, ILogger<BookingValidator> logger) : IBookingValidator
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        private readonly ILogger<BookingValidator> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public BookingValidator(IBookingRepository bookingRepository, IRoomRepository roomRepository)
            : this(bookingRepository, roomRepository, NullLogger<BookingValidator>.Instance)
        {
        }

        public async Task<BookingValidationResult> ValidateAsync(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            _logger.LogInformation("Validating booking details for room {RoomId} from {StartTime} to {EndTime}", booking.RoomId, booking.StartTime, booking.EndTime);

            var errors = new List<string>();

            var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId);
            if (room == null)
            {
                _logger.LogWarning("Booking validation failed because room {RoomId} does not exist", booking.RoomId);
                errors.Add($"Room with id {booking.RoomId} does not exist");
            }

            if (string.IsNullOrWhiteSpace(booking.Title))
            {
                _logger.LogWarning("Booking validation failed because the title was empty for room {RoomId}", booking.RoomId);
                errors.Add("Booking title cannot be empty");
            }

            if (booking.StartTime == DateTime.MinValue)
            {
                _logger.LogWarning("Booking validation failed because the start time was DateTime.MinValue");
                errors.Add("Booking start time cannot be empty");
            }

            if (booking.StartTime >= booking.EndTime)
            {
                _logger.LogWarning("Booking validation failed because the time range was invalid for room {RoomId}", booking.RoomId);
                errors.Add("Booking start time must be before end time");
            }

            if (errors.Count > 0)
            {
                return BookingValidationResult.Failure(errors);
            }

            var conflictingBooking = await _bookingRepository.GetConflictingBookingAsync(booking.RoomId, booking.StartTime, booking.EndTime, booking.Id);
            if (conflictingBooking is not null)
            {
                _logger.LogWarning("Booking validation found a conflict for room {RoomId} with booking {ConflictingBookingId}", booking.RoomId, conflictingBooking.Id);
                return BookingValidationResult.Conflict(conflictingBooking);
            }

            return BookingValidationResult.Success();
        }
    }
}
