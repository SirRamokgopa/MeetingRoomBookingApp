using MeetingRoom.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : Controller
    {

        [HttpGet]
        public ActionResult<List<RoomDTO>> GGetRooms()
        {
            return Ok();
        }
    }
}
