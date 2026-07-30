using MeetingRoom.Domain.Entities;

namespace MeetingRoom.API.Persistance
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
                new Room { Name = "Defeated Sanity Room", Capacity = 6 },
                new Room { Name = "Cannibal Corpse Room", Capacity = 8 }
            );
            context.SaveChanges();
        }
    }
}
