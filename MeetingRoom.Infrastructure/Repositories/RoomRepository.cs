using MeetingRoom.Application.Abstractions.Repositories;
using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Mappings;
using MeetingRoom.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeetingRoom.Infrastructure.Repositories
{
    public class RoomRepository(MeetingRoomContext context, ILogger<RoomRepository> logger) : IRoomRepository
    {
        private readonly MeetingRoomContext _context = context;
        private readonly ILogger<RoomRepository> _logger = logger;

        public RoomRepository(MeetingRoomContext context) : this(context, NullLogger<RoomRepository>.Instance)
        {
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving available rooms");
                var entities = await _context.Rooms.ToListAsync();
                return entities.Select(e => e.ToDomain()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve rooms");
                throw new InvalidOperationException("Unable to retrieve rooms.", ex);
            }
        }

        public async Task<Room?> GetRoomByIdAsync(int roomId)
        {
            var entity = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            return entity?.ToDomain();
        }
    }
}
