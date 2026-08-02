using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController(IBookingService bookingService, ILogger<RoomsController> logger) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly ILogger<RoomsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<List<RoomDTO>>> GetRooms()
        {
            try
            {
                var rooms = await _bookingService.GetRooms();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve rooms");
                return StatusCode(StatusCodes.Status500InternalServerError, CreateProblemDetails("Failed to retrieve rooms.", ex.Message, StatusCodes.Status500InternalServerError));
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
