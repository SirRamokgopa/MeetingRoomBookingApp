using MeetingRoom.API.DTOs;
using MeetingRoom.Domain.Entities;

namespace MeetingRoom.API.Services.Mappers
{
    public static class BookingExtentions
    {
        public static BookingDTO ToDto(this Booking booking)
        {
            return new BookingDTO
            {
                Id = booking.Id,
                Title = booking.Title,
                RoomId = booking.RoomId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };
        }

        public static Booking ToEntity(this CreateBookingDTO dto)
        {
            return new Booking
            {
                Title = dto.Title,
                RoomId = dto.RoomId,
                StartTime = (DateTime)dto.StartTime,
                EndTime = (DateTime)dto.EndTime
            };
        }
        public static void UpdateFromDto(this Booking booking, UpdateBookingDTO dto)
        {
            booking.Title = dto.Title;
            booking.RoomId = dto.RoomId;
            booking.StartTime = (DateTime)dto.StartTime;
            booking.EndTime = (DateTime)dto.EndTime;
        }
    }
}
