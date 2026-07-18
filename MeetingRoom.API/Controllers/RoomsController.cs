using MeetingRoom.API.DTOs;
using MeetingRoom.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController(IBookingService bookingService) : Controller
    {
        private readonly IBookingService _bookingService = bookingService;

        [HttpGet]
        public ActionResult<List<RoomDTO>> GetRooms()
        {
            var rooms = _bookingService.GetRooms();
            return Ok(rooms);
        }
    }
}
