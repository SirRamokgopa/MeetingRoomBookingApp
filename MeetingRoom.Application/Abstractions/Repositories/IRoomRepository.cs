using MeetingRoom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoom.Application.Abstractions.Repositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<Room>> GetAllRoomsAsync();
    }
}
