using MeetingRoom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoom.Application.Abstractions.Repositories
{
    internal interface IRoomRepository
    {
        Task<Room?> GetRoomByIdAsync(int roomId);
        Task<List<Room>> GetAllRoomsAsync();
    }
}
