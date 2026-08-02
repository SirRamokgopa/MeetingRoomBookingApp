namespace MeetingRoom.Application.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class CreateBookingDTO
    {
        public string Title  { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public class UpdateBookingDTO
    {
        public int Id { get; set; }
        public string Title  { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
