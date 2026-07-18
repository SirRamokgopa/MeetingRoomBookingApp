namespace MeetingRoom.API.DTOs
{
    public class BookingDTO
    {
        public int Id;
        public string Title = string.Empty;
        public int RoomId;
        public string Room = string.Empty;
        public DateTime StartTime;
        public DateTime EndTime;
    }

    public class CreateBookingDTO
    {
        public string Title = string.Empty;
        public int RoomId;
        public string Room = string.Empty;
        public DateTime StartTime;
        public DateTime EndTime;
    }
    public class UpdateBookingDTO
    {
        public int Id;
        public string Title = string.Empty;
        public int RoomId;
        public string Room = string.Empty;
        public DateTime StartTime;
        public DateTime EndTime;
    }
}
