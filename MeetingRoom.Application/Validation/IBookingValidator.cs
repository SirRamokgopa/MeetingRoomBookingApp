using MeetingRoom.Domain.Entities;

namespace MeetingRoom.Application.Validation
{
    /// <summary>
    /// Validates booking requests against room existence, field requirements, and scheduling conflicts.
    /// </summary>
    public interface IBookingValidator
    {
        /// <summary>
        /// Validates the supplied booking and returns the outcome as a result object.
        /// </summary>
        Task<BookingValidationResult> ValidateAsync(Booking booking);
    }
}
