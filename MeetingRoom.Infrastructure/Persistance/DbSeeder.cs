using MeetingRoom.Infrastructure.Persistance.Entities;

namespace MeetingRoom.Infrastructure.Persistance
{
    public static class DbSeeder
    {
        public static void SeedDb(MeetingRoomContext context)
        {
            if (context.Rooms.Any())
            {
                return;
            }
            context.Rooms.AddRange(
                new RoomEntity { Name = "Defeated Sanity Room", Capacity = 6 },
                new RoomEntity { Name = "Cannibal Corpse Room", Capacity = 8 }
            );
            context.SaveChanges();
        }
    }
}
