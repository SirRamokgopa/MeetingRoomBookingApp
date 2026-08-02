using MeetingRoom.Application.Abstractions.Repositories;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Services;
using MeetingRoom.Application.Validation;
using MeetingRoom.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeetingRoom.Application.Services
{
    /// <summary>
    /// Provides business operations for booking management.
    /// </summary>
    public class BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IBookingValidator bookingValidator, ILogger<BookingService> logger) : IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        private readonly IBookingValidator _bookingValidator = bookingValidator ?? throw new ArgumentNullException(nameof(bookingValidator));
        private readonly ILogger<BookingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IBookingValidator bookingValidator)
            : this(bookingRepository, roomRepository, bookingValidator, NullLogger<BookingService>.Instance)
        {
        }

        /// <summary>
        /// Creates a new booking after validating the request.
        /// </summary>
        public async Task<BookingDTO> CreateBooking(CreateBookingDTO booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            _logger.LogInformation("Creating booking for room {RoomId} from {StartTime} to {EndTime}", booking.RoomId, booking.StartTime, booking.EndTime);

            try
            {
                var bookingEntity = booking.ToEntity();

                var validationResult = await _bookingValidator.ValidateAsync(bookingEntity);
                EnsureValid(validationResult);

                await _bookingRepository.AddAsync(bookingEntity);

                _logger.LogInformation("Booking {BookingId} created successfully", bookingEntity.Id);
                return bookingEntity.ToDto();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Booking validation failed for room {RoomId}", booking.RoomId);
                throw;
            }
            catch (BookingConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating booking for room {RoomId}", booking.RoomId);
                throw new InvalidOperationException("An unexpected error occurred while creating the booking.", ex);
            }
        }

        /// <summary>
        /// Updates an existing booking with the supplied values.
        /// </summary>
        public async Task<BookingDTO> UpdateBooking(UpdateBookingDTO booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            _logger.LogInformation("Updating booking {BookingId} for room {RoomId}", booking.Id, booking.RoomId);

            try
            {
                var bookingFromDb = await _bookingRepository.GetBookingAsync(booking.Id);

                if (bookingFromDb == null)
                {
                    _logger.LogWarning("Booking {BookingId} was not found for update", booking.Id);
                    throw new ArgumentException($"Booking with id {booking.Id} was not found", nameof(booking));
                }

                bookingFromDb.UpdateFromDto(booking);

                var validationResult = await _bookingValidator.ValidateAsync(bookingFromDb);
                EnsureValid(validationResult);

                await _bookingRepository.UpdateAsync(bookingFromDb);
                _logger.LogInformation("Booking {BookingId} updated successfully", bookingFromDb.Id);
                return bookingFromDb.ToDto();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Booking update validation failed for booking {BookingId}", booking.Id);
                throw;
            }
            catch (BookingConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating booking {BookingId}", booking.Id);
                throw new InvalidOperationException("An unexpected error occurred while updating the booking.", ex);
            }
        }

        /// <summary>
        /// Deletes an existing booking by identifier.
        /// </summary>
        public async Task DeleteBookingAsync(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bookingId));
            }

            _logger.LogInformation("Deleting booking {BookingId}", bookingId);

            try
            {
                var booking = await _bookingRepository.GetBookingAsync(bookingId);

                if (booking == null)
                {
                    _logger.LogWarning("Delete requested for missing booking {BookingId}", bookingId);
                    return;
                }

                await _bookingRepository.DeleteAsync(booking);
                _logger.LogInformation("Booking {BookingId} deleted successfully", bookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting booking {BookingId}", bookingId);
                throw new InvalidOperationException("An unexpected error occurred while deleting the booking.", ex);
            }
        }

        /// <summary>
        /// Retrieves all bookings.
        /// </summary>
        public async Task<IEnumerable<BookingDTO>> GetBookings()
        {
            try
            {
                _logger.LogInformation("Retrieving all bookings");
                var bookings = await _bookingRepository.GetAllBookingAsync();
                var bookingDtos = bookings.Select(b => b.ToDto()).ToList();
                _logger.LogInformation("Retrieved {BookingCount} bookings", bookingDtos.Count);
                return bookingDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve bookings");
                throw new InvalidOperationException("Unable to retrieve bookings.", ex);
            }
        }

        /// <summary>
        /// Retrieves a booking by identifier.
        /// </summary>
        public async Task<BookingDTO?> GetBookingById(int bookingId)
        {
            _logger.LogInformation("Retrieving booking {BookingId}", bookingId);

            try
            {
                var booking = await _bookingRepository.GetBookingAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} was not found", bookingId);
                }

                return booking?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve booking {BookingId}", bookingId);
                throw new InvalidOperationException("Unable to retrieve the booking.", ex);
            }
        }

        /// <summary>
        /// Retrieves the available rooms.
        /// </summary>
        public async Task<IEnumerable<RoomDTO>> GetRooms()
        {
            try
            {
                _logger.LogInformation("Retrieving available rooms");
                var rooms = await _roomRepository.GetAllRoomsAsync();
                return rooms.Select(r => new RoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve rooms");
                throw new InvalidOperationException("Unable to retrieve rooms.", ex);
            }
        }

        /// <summary>
        /// Translates a <see cref="BookingValidationResult"/> into the service's existing exception contract,
        /// so callers (e.g. controllers) can keep distinguishing validation failures (400) from conflicts (409).
        /// </summary>
        private static void EnsureValid(BookingValidationResult validationResult)
        {
            if (validationResult.IsValid)
            {
                return;
            }

            if (validationResult.ConflictingBooking is not null)
            {
                throw new BookingConflictException(validationResult.ConflictingBooking);
            }

            throw new ArgumentException(string.Join(" ", validationResult.Errors));
        }
    }

    /// <summary>
    /// Represents a booking conflict detected during validation.
    /// </summary>
    public class BookingConflictException : Exception
    {
        /// <summary>
        /// Gets the identifier of the conflicting booking.
        /// </summary>
        public int ConflictingBookingId { get; }

        /// <summary>
        /// Gets the room identifier involved in the conflict.
        /// </summary>
        public int RoomId { get; }

        /// <summary>
        /// Gets the conflicting booking start time.
        /// </summary>
        public DateTime ConflictingStartTime { get; }

        /// <summary>
        /// Gets the conflicting booking end time.
        /// </summary>
        public DateTime ConflictingEndTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingConflictException"/> class.
        /// </summary>
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
