using MeetingRoom.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController : Controller
    {
        [HttpGet]
        public ActionResult<List<BookingDTO>> GetBookings()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<BookingDTO> GetBookingFromId()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult AddBooking()
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult EditBooking()
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBooking()
        {
            return Ok();
        }
    }
}
