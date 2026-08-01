using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Persistance.Entities;

namespace MeetingRoom.Infrastructure.Mappings
{
    /// <summary>
    /// Translates rooms between the domain model and the EF Core persistence model.
    /// </summary>
    internal static class RoomMapper
    {
        public static Room ToDomain(this RoomEntity entity)
        {
            return new Room
            {
                Id = entity.Id,
                Name = entity.Name,
                Capacity = entity.Capacity
            };
        }

        public static RoomEntity ToEntity(this Room room)
        {
            return new RoomEntity
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity
            };
        }
    }
}
