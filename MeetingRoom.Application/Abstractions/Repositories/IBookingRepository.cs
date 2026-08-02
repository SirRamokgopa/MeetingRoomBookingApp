using MeetingRoom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoom.Application.Abstractions.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingAsync(int id);
        Task<IEnumerable<Booking>> GetAllBookingAsync();
        Task AddAsync(Booking newBooking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<Booking?> GetConflictingBookingAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    }
}
