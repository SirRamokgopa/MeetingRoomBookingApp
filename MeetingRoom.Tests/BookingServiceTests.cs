using Microsoft.EntityFrameworkCore;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Persistance;
using MeetingRoom.Infrastructure.Persistance.Entities;
using MeetingRoom.Infrastructure.Repositories;
using MeetingRoom.Application.Services;

namespace MeetingRoom.Tests
{
    [TestClass]
    public sealed class BookingServiceTests
    {
        private MeetingRoomContext _context;
        private IBookingService _bookingService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<MeetingRoomContext>()
                .UseInMemoryDatabase(databaseName: $"MeetingRoomDb_{Guid.NewGuid()}")
                .Options;

            _context = new MeetingRoomContext(options);
            
            // Seed test data
            SeedTestData();
            
            // Initialize the booking service via the repositories, matching the production DI wiring
            var bookingRepository = new BookingRepository(_context);
            var roomRepository = new RoomRepository(_context);
            _bookingService = new BookingService(bookingRepository, roomRepository);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context?.Dispose();
        }

        private void SeedTestData()
        {
            var room1 = new RoomEntity { Id = 1, Name = "Party Room A", Capacity = 10 };
            var room2 = new RoomEntity { Id = 2, Name = "Sleepy Meeting Room B", Capacity = 6 };
            
            _context.Rooms.AddRange(room1, room2);
            _context.SaveChanges();
        }

        #region GetRooms Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetRooms_ReturnsAllRooms()
        {
            // Act
            var rooms = await _bookingService.GetRooms();

            // Assert
            Assert.IsNotNull(rooms);
            Assert.AreEqual(2, rooms.Count());
        }

        [TestMethod]
        public async Task GetRooms_ReturnsRoomsWithCorrectData()
        {
            // Act
            var rooms = (List<RoomDTO>) await _bookingService.GetRooms();

            // Assert
            Assert.AreEqual("Party Room A", rooms[0].Name);
            Assert.AreEqual(10, rooms[0].Capacity);
            Assert.AreEqual("Sleepy Meeting Room B", rooms[1].Name);
            Assert.AreEqual(6, rooms[1].Capacity);
        }

        #endregion

        #region ValidateBooking Tests
        //===================================================================================================================

        [TestMethod]
        public async Task ValidateBooking_ThrowsException_WhenRoomDoesNotExist()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "What? Another Team Meeting?",
                RoomId = 69, // Where the hell is this??
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.ValidateBooking(booking));
        }

        [TestMethod]
        public async Task ValidateBooking_ThrowsException_WhenStartTimeAfterEndTime()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "Should have been an email Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1) // End time before start time
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.ValidateBooking(booking));
        }

        [TestMethod]
        public async Task ValidateBooking_ThrowsException_WhenBookingTitleIsEmpty()
        {
            // Arrange
            var booking = new Booking
            {
                Title = string.Empty,
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.ValidateBooking(booking));
        }

        [TestMethod]
        public async Task ValidateBooking_ThrowsException_WhenBookingStartTimeIsMinValue()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "Meeting before time",
                RoomId = 1,
                StartTime = DateTime.MinValue,
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.ValidateBooking(booking));
        }

        #endregion

        #region CreateBooking Tests
        //===================================================================================================================

        [TestMethod]
        public async Task CreateBooking_SuccessfullyCreatesBooking()
        {
            // Arrange
            var booking = new CreateBookingDTO
            {
                Title = "Boring Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            await _bookingService.CreateBooking(booking);
            var bookings = await _bookingService.GetBookings();

            // Assert
            Assert.IsNotEmpty(bookings);
        }

        [TestMethod]
        public async Task CreateBooking_ThrowsException_WhenBookingConflicts()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            
            // Create first booking
            var booking1 = new BookingEntity
            {
                Title = "Meeting on meetings efficiency",
                RoomId = 1,
                StartTime = startTime,
                EndTime = endTime
            };
            
            _context.Bookings.Add(booking1);
            _context.SaveChanges();

            // Act & Assert - Try to create overlapping booking
            var booking2 = new CreateBookingDTO
            {
                Title = "Discussion on why so many Meetings",
                RoomId = 1,
                StartTime = startTime.AddMinutes(30), // Overlaps with first booking, Ayeye!
                EndTime = endTime.AddHours(1)
            };

            // I'm thinking we should throw a confict exception here so that we know what's going on, but for now let's just throw an ArgumentException
            await Assert.ThrowsAsync<BookingConflictException>(async () => await _bookingService.CreateBooking(booking2));
        }

        [TestMethod]
        public async Task BookingConflict_IsNotDetected_WhenBookingsAreInDifferentRooms()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(1).Date.AddHours(10);
            var endTime = startTime.AddHours(1);

            var booking1 = new BookingEntity
            {
                Title = "Meeting about last meeting",
                RoomId = 1,
                StartTime = startTime,
                EndTime = endTime
            };

            _context.Bookings.Add(booking1);
            _context.SaveChanges();

            // Act - Same time, different room
            var booking2 = new CreateBookingDTO
            {
                Title = "I'm double booked",
                RoomId = 2,
                StartTime = startTime,
                EndTime = endTime
            };

            // Assert - Should not throw
            await _bookingService.CreateBooking(booking2);
        }

        [TestMethod]
        public async Task CreateBooking_ThrowsException_WhenBookingHasNoStartTime()
        {
            // Arrange
            var booking = new CreateBookingDTO
            {
                Title = "Coffee Meeting",
                RoomId = 1,
                StartTime = DateTime.MinValue,
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.CreateBooking(booking));
        }

        #endregion

        #region GetBookings Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetBookings_ReturnsEmptyList_WhenNoBookingsExist()
        {
            // Act
            var bookings = await _bookingService.GetBookings();

            // Assert
            Assert.IsNotNull(bookings);
            Assert.AreEqual(0, bookings.Count());
        }

        [TestMethod]
        public async Task GetBookings_ReturnsAllBookings_WhenBookingsExist()
        {
            // Arrange
            var booking1 = new BookingEntity
            {
                Title = "Why are managers like this Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };
            
            var booking2 = new BookingEntity
            {
                Title = "Meeting to interrupt devs' flow state",
                RoomId = 2,
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now.AddHours(4)
            };

            _context.Bookings.AddRange(booking1, booking2);
            _context.SaveChanges();

            // Act
            var bookings = (List<BookingDTO>) await _bookingService.GetBookings();

            // Assert
            Assert.HasCount(2, bookings);
            //** I also wanna check here that bookings are ordered by start time  
            Assert.IsTrue(bookings.Any(b => b.Title == "Why are managers like this Meeting"));
            Assert.IsTrue(bookings.Any(b => b.Title == "Meeting to interrupt devs' flow state"));
        }

        #endregion

        #region GetBookingById Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetBookingById_ReturnsCorrectBooking()
        {
            // Arrange
            var booking = new BookingEntity
            {
                Title = "Team Synergy Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Act
            var retrievedBooking = await _bookingService.GetBookingById(booking.Id);

            // Assert
            Assert.IsNotNull(retrievedBooking);
            Assert.AreEqual("Team Synergy Meeting", retrievedBooking.Title);
            Assert.AreEqual(1, retrievedBooking.RoomId);
        }

        [TestMethod]
        public async Task GetBookingById_ReturnsNull_WhenBookingDoesNotExist()
        {
            // Act
            var booking = await _bookingService.GetBookingById(999);

            // Assert
            Assert.IsNull(booking);
        }

        #endregion

        #region UpdateBooking Tests
        //===================================================================================================================

        [TestMethod]
        public async Task UpdateBooking_SuccessfullyUpdatesBooking()
        {
            // Arrange
            var originalBooking = new BookingEntity
            {
                Id = 420, 
                Title = "Original Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            _context.Bookings.Add(originalBooking);
            _context.SaveChanges();

            var bookingId = originalBooking.Id;

            var bookingUpdate = new UpdateBookingDTO
            {
                Id = 420,
                Title = "New Improved Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };


            // Act
            var updatedBooking = await _bookingService.UpdateBooking(bookingUpdate);
            var updatedBookingFromGet = await _bookingService.GetBookingById(bookingId);

            // Assert
            Assert.IsNotNull(updatedBooking);
            Assert.AreEqual(bookingId, updatedBooking.Id);
            Assert.IsNotNull(updatedBookingFromGet);
            Assert.AreEqual("New Improved Meeting", updatedBookingFromGet.Title);
        }

        #endregion

        #region DeleteBooking Tests
        //===================================================================================================================

        [TestMethod]
        public async Task DeleteBooking_SuccessfullyDeletesBooking()
        {
            // Arrange
            var booking = new BookingEntity
            {
                Title = "We didn't need this meeting anyways",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var bookingId = booking.Id;

            // Act
            await _bookingService.DeleteBookingAsync(bookingId);
            var deletedBooking = await _bookingService.GetBookingById(bookingId);

            // Assert
            Assert.IsNull(deletedBooking);
        }

        [TestMethod]
        public async Task DeleteBooking_DoesNotThrowException_WhenBookingDoesNotExist()
        {
            // Act & Assert - Should not throw
            await _bookingService.DeleteBookingAsync(666);
        }

        #endregion

    }
}
