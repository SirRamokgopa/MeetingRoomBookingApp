using MeetingRoom.Domain.Entities;

namespace MeetingRoom.Application.Validation
{
    /// <summary>
    /// Represents the outcome of validating a booking request without relying on exceptions for control flow.
    /// </summary>
    public class BookingValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the booking passed validation.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the collection of validation error messages. Empty when <see cref="IsValid"/> is <c>true</c>
        /// or when the failure was caused by a scheduling conflict.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Gets the existing booking that conflicts with the requested time slot, if any.
        /// </summary>
        public Booking? ConflictingBooking { get; }

        private BookingValidationResult(bool isValid, IReadOnlyList<string> errors, Booking? conflictingBooking)
        {
            IsValid = isValid;
            Errors = errors;
            ConflictingBooking = conflictingBooking;
        }

        /// <summary>
        /// Creates a result representing a booking that passed all validation checks.
        /// </summary>
        public static BookingValidationResult Success() => new(true, Array.Empty<string>(), null);

        /// <summary>
        /// Creates a result representing one or more failed validation rules.
        /// </summary>
        public static BookingValidationResult Failure(IEnumerable<string> errors) =>
            new(false, errors.ToList(), null);

        /// <summary>
        /// Creates a result representing a scheduling conflict with an existing booking.
        /// </summary>
        public static BookingValidationResult Conflict(Booking conflictingBooking) =>
            new(false, Array.Empty<string>(), conflictingBooking);
    }
}
