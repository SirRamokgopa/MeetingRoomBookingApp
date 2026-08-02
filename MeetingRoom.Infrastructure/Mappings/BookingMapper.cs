using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Persistance.Entities;

namespace MeetingRoom.Infrastructure.Mappings
{
    /// <summary>
    /// Translates bookings between the domain model and the EF Core persistence model.
    /// </summary>
    internal static class BookingMapper
    {
        public static Booking ToDomain(this BookingEntity entity)
        {
            return new Booking
            {
                Id = entity.Id,
                Title = entity.Title,
                RoomId = entity.RoomId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        public static BookingEntity ToEntity(this Booking booking)
        {
            return new BookingEntity
            {
                Id = booking.Id,
                Title = booking.Title,
                RoomId = booking.RoomId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };
        }

        public static void UpdateFromDomain(this BookingEntity entity, Booking booking)
        {
            entity.Title = booking.Title;
            entity.RoomId = booking.RoomId;
            entity.StartTime = booking.StartTime;
            entity.EndTime = booking.EndTime;
        }
    }
}
