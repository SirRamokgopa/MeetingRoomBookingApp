using Microsoft.EntityFrameworkCore;
using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Persistance;
using MeetingRoom.Infrastructure.Persistance.Entities;
using MeetingRoom.Infrastructure.Repositories;

namespace MeetingRoom.Tests
{
    [TestClass]
    public sealed class BookingRepositoryTests
    {
        private MeetingRoomContext _context;
        private BookingRepository _bookingRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<MeetingRoomContext>()
                .UseInMemoryDatabase(databaseName: $"MeetingRoomDb_{Guid.NewGuid()}")
                .Options;

            _context = new MeetingRoomContext(options);
            _bookingRepository = new BookingRepository(_context);

            SeedRooms();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context?.Dispose();
        }

        private void SeedRooms()
        {
            _context.Rooms.AddRange(
                new RoomEntity { Id = 1, Name = "Party Room A", Capacity = 10 },
                new RoomEntity { Id = 2, Name = "Sleepy Meeting Room B", Capacity = 6 });
            _context.SaveChanges();
        }

        #region AddAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task AddAsync_PersistsBooking_AndAssignsId()
        {
            // Arrange
            var booking = new Booking
            {
                Title = "Yet Another Standup",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act
            await _bookingRepository.AddAsync(booking);

            // Assert
            Assert.AreNotEqual(0, booking.Id);
            var persisted = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual("Yet Another Standup", persisted.Title);
        }

        #endregion

        #region GetAllBookingAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetAllBookingAsync_ReturnsEmptyList_WhenNoBookingsExist()
        {
            // Act
            var bookings = await _bookingRepository.GetAllBookingAsync();

            // Assert
            Assert.IsNotNull(bookings);
            Assert.AreEqual(0, bookings.Count());
        }

        [TestMethod]
        public async Task GetAllBookingAsync_ReturnsAllBookings_WhenBookingsExist()
        {
            // Arrange
            _context.Bookings.AddRange(
                new BookingEntity { Title = "Retro That Could Have Been an Email", RoomId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2) },
                new BookingEntity { Title = "Sprint Planning Purgatory", RoomId = 2, StartTime = DateTime.Now.AddHours(3), EndTime = DateTime.Now.AddHours(4) });
            _context.SaveChanges();

            // Act
            var bookings = (await _bookingRepository.GetAllBookingAsync()).ToList();

            // Assert
            Assert.HasCount(2, bookings);
            Assert.IsTrue(bookings.Any(b => b.Title == "Retro That Could Have Been an Email"));
            Assert.IsTrue(bookings.Any(b => b.Title == "Sprint Planning Purgatory"));
        }

        #endregion

        #region GetBookingAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetBookingAsync_ReturnsCorrectBooking_WhenBookingExists()
        {
            // Arrange
            var entity = new BookingEntity { Title = "Budget Review", RoomId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2) };
            _context.Bookings.Add(entity);
            _context.SaveChanges();

            // Act
            var result = await _bookingRepository.GetBookingAsync(entity.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Budget Review", result.Title);
            Assert.AreEqual(1, result.RoomId);
        }

        [TestMethod]
        public async Task GetBookingAsync_ReturnsNull_WhenBookingDoesNotExist()
        {
            // Act
            var result = await _bookingRepository.GetBookingAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetConflictingBookingAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetConflictingBookingAsync_ReturnsBooking_WhenTimesOverlap()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            var entity = new BookingEntity { Title = "Meeting About Meetings", RoomId = 1, StartTime = startTime, EndTime = endTime };
            _context.Bookings.Add(entity);
            _context.SaveChanges();

            // Act
            var result = await _bookingRepository.GetConflictingBookingAsync(1, startTime.AddMinutes(30), endTime.AddHours(1));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entity.Id, result.Id);
        }

        [TestMethod]
        public async Task GetConflictingBookingAsync_ReturnsNull_WhenNoOverlapExists()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            _context.Bookings.Add(new BookingEntity { Title = "Meeting About Meetings", RoomId = 1, StartTime = startTime, EndTime = endTime });
            _context.SaveChanges();

            // Act
            var result = await _bookingRepository.GetConflictingBookingAsync(1, endTime.AddHours(1), endTime.AddHours(2));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetConflictingBookingAsync_ReturnsNull_WhenOverlapIsInDifferentRoom()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            _context.Bookings.Add(new BookingEntity { Title = "Meeting About Meetings", RoomId = 1, StartTime = startTime, EndTime = endTime });
            _context.SaveChanges();

            // Act
            var result = await _bookingRepository.GetConflictingBookingAsync(2, startTime, endTime);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetConflictingBookingAsync_ExcludesGivenBookingId()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(2);
            var entity = new BookingEntity { Title = "Meeting About Meetings", RoomId = 1, StartTime = startTime, EndTime = endTime };
            _context.Bookings.Add(entity);
            _context.SaveChanges();

            // Act - excluding the only overlapping booking should yield no conflict
            var result = await _bookingRepository.GetConflictingBookingAsync(1, startTime, endTime, entity.Id);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region UpdateAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task UpdateAsync_UpdatesExistingBooking()
        {
            // Arrange
            var entity = new BookingEntity { Title = "Original Meeting", RoomId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2) };
            _context.Bookings.Add(entity);
            _context.SaveChanges();

            var updated = new Booking
            {
                Id = entity.Id,
                Title = "New Improved Meeting",
                RoomId = 2,
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now.AddHours(4)
            };

            // Act
            await _bookingRepository.UpdateAsync(updated);

            // Assert
            var persisted = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == entity.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual("New Improved Meeting", persisted.Title);
            Assert.AreEqual(2, persisted.RoomId);
        }

        [TestMethod]
        public async Task UpdateAsync_DoesNotThrow_WhenBookingDoesNotExist()
        {
            // Arrange
            var missingBooking = new Booking
            {
                Id = 999,
                Title = "Ghost Meeting",
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            // Act & Assert - should be a no-op, not throw
            await _bookingRepository.UpdateAsync(missingBooking);
        }

        #endregion

        #region DeleteAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task DeleteAsync_RemovesExistingBooking()
        {
            // Arrange
            var entity = new BookingEntity { Title = "We didn't need this meeting anyways", RoomId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2) };
            _context.Bookings.Add(entity);
            _context.SaveChanges();

            var booking = new Booking { Id = entity.Id };

            // Act
            await _bookingRepository.DeleteAsync(booking);

            // Assert
            var persisted = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == entity.Id);
            Assert.IsNull(persisted);
        }

        [TestMethod]
        public async Task DeleteAsync_DoesNotThrow_WhenBookingDoesNotExist()
        {
            // Arrange
            var booking = new Booking { Id = 999 };

            // Act & Assert - should be a no-op, not throw
            await _bookingRepository.DeleteAsync(booking);
        }

        #endregion
    }
}
