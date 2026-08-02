using MeetingRoom.Infrastructure.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Infrastructure.Persistance
{
    public class MeetingRoomContext: DbContext
    {
        public MeetingRoomContext(DbContextOptions<MeetingRoomContext> options) : base(options)
        {
        }

        public DbSet<BookingEntity> Bookings { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomEntity>(entity =>
            {
                entity.ToTable("Rooms");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired();
            });

            modelBuilder.Entity<BookingEntity>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired();

                entity.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}