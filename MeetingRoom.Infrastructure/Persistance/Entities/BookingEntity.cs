namespace MeetingRoom.Infrastructure.Persistance.Entities
{
    /// <summary>
    /// EF Core persistence model for a booking. Kept separate from the domain
    /// entity so the domain layer has no dependency on EF Core.
    /// </summary>
    public class BookingEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public RoomEntity Room { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
