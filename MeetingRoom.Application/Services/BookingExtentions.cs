using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Entities;

namespace MeetingRoom.Application.Services
{
    public static class BookingExtentions
    {
        public static BookingDTO ToDto(this Booking booking, string roomName)
        {
            return new BookingDTO
            {
                Id = booking.Id,
                Title = booking.Title,
                RoomId = booking.RoomId,
                RoomName = roomName,
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
                StartTime = dto.StartTime!.Value,
                EndTime = dto.EndTime!.Value
            };
        }
        public static void UpdateFromDto(this Booking booking, UpdateBookingDTO dto)
        {
            booking.Title = dto.Title;
            booking.RoomId = dto.RoomId;
            booking.StartTime = dto.StartTime!.Value;
            booking.EndTime = dto.EndTime!.Value;
        }
    }
}
