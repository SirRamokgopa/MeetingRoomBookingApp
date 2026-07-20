namespace MeetingRoom.API.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
