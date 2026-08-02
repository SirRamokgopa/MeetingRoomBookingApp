namespace MeetingRoom.Infrastructure.Persistance.Entities
{
    /// <summary>
    /// EF Core persistence model for a room. Kept separate from the domain
    /// entity so the domain layer has no dependency on EF Core.
    /// </summary>
    public class RoomEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }

        public List<BookingEntity> Bookings { get; set; } = new();
    }
}
