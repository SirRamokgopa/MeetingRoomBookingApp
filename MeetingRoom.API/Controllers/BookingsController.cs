using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController(IBookingService bookingService, ILogger<BookingsController> logger) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly ILogger<BookingsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<List<BookingDTO>>> GetBookings()
        {
            try
            {
                var bookings = await _bookingService.GetBookings();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve bookings");
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Failed to retrieve bookings.", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(id);

                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} was not found", id);
                    return NotFound(CreateProblemDetails("Booking not found.", $"Booking with id {id} was not found.", StatusCodes.Status404NotFound));
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve booking {BookingId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Failed to retrieve booking.", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] CreateBookingDTO booking)
        {
            if (booking == null)
                return BadRequest(CreateProblemDetails("Invalid booking request.", "A booking object is required.", StatusCodes.Status400BadRequest));

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var createdBooking = await _bookingService.CreateBooking(booking);

                return CreatedAtAction(
                    nameof(GetBooking),
                    new { id = createdBooking.Id },
                    createdBooking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while creating booking for room {RoomId}", booking.RoomId);
                return BadRequest(CreateProblemDetails("Invalid booking request.", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (BookingConflictException ex)
            {
                _logger.LogWarning(ex, "Booking conflict while creating booking for room {RoomId}", booking.RoomId);
                return Conflict(CreateProblemDetails("Booking conflict.", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating booking for room {RoomId}", booking.RoomId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Unable to create booking.", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDTO>> UpdateBooking(int id, [FromBody] UpdateBookingDTO booking)
        {
            if (booking == null)
                return BadRequest(CreateProblemDetails("Invalid booking request.", "A booking object is required.", StatusCodes.Status400BadRequest));

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (booking.Id != id)
            {
                return BadRequest(CreateProblemDetails("Invalid booking request.", "The booking id in the URL must match the booking id in the body.", StatusCodes.Status400BadRequest));
            }

            try
            {
                var existingBooking = await _bookingService.GetBookingById(id);
                if (existingBooking == null)
                {
                    _logger.LogWarning("Update requested for missing booking {BookingId}", id);
                    return NotFound(CreateProblemDetails("Booking not found.", $"Booking with id {id} was not found.", StatusCodes.Status404NotFound));
                }

                var updatedBooking = await _bookingService.UpdateBooking(booking);
                return Ok(updatedBooking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while updating booking {BookingId}", id);
                return BadRequest(CreateProblemDetails("Invalid booking request.", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (BookingConflictException ex)
            {
                _logger.LogWarning(ex, "Booking conflict while updating booking {BookingId}", id);
                return Conflict(CreateProblemDetails("Booking conflict.", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating booking {BookingId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Unable to update booking.", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(id);
                if (booking == null)
                {
                    // Note to self: Look into what might happen if two clients try to delete the same booking at the same time. 
                    _logger.LogWarning("Delete requested for missing booking {BookingId}", id);
                    return NotFound(CreateProblemDetails("Booking not found.", $"Booking with id {id} was not found.", StatusCodes.Status404NotFound));
                }

                await _bookingService.DeleteBookingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting booking {BookingId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Unable to delete booking.", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        private static ProblemDetails CreateProblemDetails(string title, string detail, int statusCode)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode
            };
        }
    }
}
