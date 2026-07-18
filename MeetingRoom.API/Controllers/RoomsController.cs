using MeetingRoom.API.DTOs;
using MeetingRoom.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController(IBookingService bookingService) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

        [HttpGet]
        public async Task<ActionResult<List<RoomDTO>>> GetRooms()
        {
            var rooms = await _bookingService.GetRooms();
            return Ok(rooms);
        }
    }
}
