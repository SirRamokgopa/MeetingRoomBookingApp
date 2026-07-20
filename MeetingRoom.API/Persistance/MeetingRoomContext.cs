using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.API.Persistance
{
    public class MeetingRoomContext: DbContext
    {
        public MeetingRoomContext(DbContextOptions<MeetingRoomContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}