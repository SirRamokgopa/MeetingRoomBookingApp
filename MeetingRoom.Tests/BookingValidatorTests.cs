using Microsoft.EntityFrameworkCore;
using MeetingRoom.Application.Validation;
using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Persistance;
using MeetingRoom.Infrastructure.Persistance.Entities;
using MeetingRoom.Infrastructure.Repositories;

namespace MeetingRoom.Tests
{
    [TestClass]
    public sealed class BookingValidatorTests
    {
        private MeetingRoomContext _context;
        private IBookingValidator _bookingValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<MeetingRoomContext>()
                .UseInMemoryDatabase(databaseName: $"MeetingRoomDb_{Guid.NewGuid()}")
                .Options;

            _context = new MeetingRoomContext(options);

            var bookingRepository = new BookingRepository(_context);
            var roomRepository = new RoomRepository(_context);
            _bookingValidator = new BookingValidator(bookingRepository, roomRepository);

            _context.Rooms.Add(new RoomEntity { Id = 1, Name = "Party Room A", Capacity = 10 });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context?.Dispose();
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsSuccess_WhenBookingIsValid()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "A Perfectly Reasonable Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
            Assert.IsNull(result.ConflictingBooking);
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsError_WhenRoomDoesNotExist()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "What? Another Team Meeting?",
                RoomId = 69, // Where the hell is this??
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.ConflictingBooking);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("does not exist")));
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsError_WhenStartTimeAfterEndTime()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "Should have been an email Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1) // End time before start time
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("start time must be before end time")));
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsError_WhenBookingTitleIsEmpty()
        {
            // Arrange
            var booking = new Booking
            {
                Title = string.Empty,
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("title cannot be empty")));
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsError_WhenBookingStartTimeIsMinValue()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "Meeting before time",
                RoomId = 1,
                StartTime = DateTime.MinValue,
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.Contains("start time cannot be empty")));
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsAllErrors_WhenMultipleRulesFail()
        {
            // Arrange - empty title AND invalid time range in the same request
            var booking = new Booking
            {
                Title = string.Empty,
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(booking);

            // Assert - both failures are reported together, not just the first one
            Assert.IsFalse(result.IsValid);
            Assert.HasCount(2, result.Errors);
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsConflict_WhenTimesOverlapInSameRoom()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            var existingBooking = new BookingEntity { Title = "Meeting on meetings efficiency", RoomId = 1, StartTime = startTime, EndTime = endTime };
            _context.Bookings.Add(existingBooking);
            _context.SaveChanges();

            var newBooking = new Booking
            {
                Title = "Discussion on why so many Meetings",
                RoomId = 1,
                StartTime = startTime.AddMinutes(30),
                EndTime = endTime.AddHours(1)
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(newBooking);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.ConflictingBooking);
            Assert.AreEqual(existingBooking.Id, result.ConflictingBooking.Id);
            Assert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public async Task ValidateAsync_ReturnsSuccess_WhenOverlapIsInDifferentRoom()
        {
            // Arrange
            _context.Rooms.Add(new RoomEntity { Id = 2, Name = "Sleepy Meeting Room B", Capacity = 6 });
            var startTime = DateTime.Now.AddDays(1).Date.AddHours(10);
            var endTime = startTime.AddHours(1);
            _context.Bookings.Add(new BookingEntity { Title = "Meeting about last meeting", RoomId = 1, StartTime = startTime, EndTime = endTime });
            _context.SaveChanges();

            var newBooking = new Booking
            {
                Title = "I'm double booked",
                RoomId = 2,
                StartTime = startTime,
                EndTime = endTime
            };

            // Act
            var result = await _bookingValidator.ValidateAsync(newBooking);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}
