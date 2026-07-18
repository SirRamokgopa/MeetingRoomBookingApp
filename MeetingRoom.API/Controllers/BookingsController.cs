using MeetingRoom.API.DTOs;
using MeetingRoom.API.Persistance;
using MeetingRoom.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController(IBookingService bookingService) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

        [HttpGet]
        public async Task<ActionResult<List<BookingDTO>>> GetBookings()
        {
            var bookings = await _bookingService.GetBookings();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingById(id);

            if (booking == null) return NotFound();

            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] CreateBookingDTO booking)
        {
            if (booking == null) return BadRequest("A booking object is reqired");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBooking = await _bookingService.CreateBooking(booking);

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = createdBooking.Id },
                createdBooking
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDTO>> UpdateBooking(int id, [FromBody] UpdateBookingDTO booking)
        {
            if (booking == null) return BadRequest("A booking object is reqired");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var _ = await _bookingService.GetBookingById(id);

            var updatedBooking = await _bookingService.UpdateBooking(booking);

            return Ok(updatedBooking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingService.GetBookingById(id);

            if (booking == null) return NotFound("Booking does not exist");

            await _bookingService.DeleteBookingAsync(id);
            return NoContent();
        }
    }
}
