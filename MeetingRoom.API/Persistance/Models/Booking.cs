namespace MeetingRoom.API.Persistance
{
    public class Booking
    {
        public int Id { get; set; }  
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
