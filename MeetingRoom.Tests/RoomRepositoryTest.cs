using Microsoft.EntityFrameworkCore;
using MeetingRoom.Infrastructure.Persistance;
using MeetingRoom.Infrastructure.Persistance.Entities;
using MeetingRoom.Infrastructure.Repositories;

namespace MeetingRoom.Tests
{
    [TestClass]
    public sealed class RoomRepositoryTest
    {
        private MeetingRoomContext _context;
        private RoomRepository _roomRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<MeetingRoomContext>()
                .UseInMemoryDatabase(databaseName: $"MeetingRoomDb_{Guid.NewGuid()}")
                .Options;

            _context = new MeetingRoomContext(options);
            _roomRepository = new RoomRepository(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context?.Dispose();
        }

        #region GetAllRoomsAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetAllRoomsAsync_ReturnsEmptyList_WhenNoRoomsExist()
        {
            // Act
            var rooms = await _roomRepository.GetAllRoomsAsync();

            // Assert
            Assert.IsNotNull(rooms);
            Assert.AreEqual(0, rooms.Count());
        }

        [TestMethod]
        public async Task GetAllRoomsAsync_ReturnsAllRooms_WhenRoomsExist()
        {
            // Arrange
            _context.Rooms.AddRange(
                new RoomEntity { Name = "Party Room A", Capacity = 10 },
                new RoomEntity { Name = "Sleepy Meeting Room B", Capacity = 6 });
            _context.SaveChanges();

            // Act
            var rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();

            // Assert
            Assert.HasCount(2, rooms);
            Assert.IsTrue(rooms.Any(r => r.Name == "Party Room A" && r.Capacity == 10));
            Assert.IsTrue(rooms.Any(r => r.Name == "Sleepy Meeting Room B" && r.Capacity == 6));
        }

        #endregion

        #region GetRoomByIdAsync Tests
        //===================================================================================================================

        [TestMethod]
        public async Task GetRoomByIdAsync_ReturnsCorrectRoom_WhenRoomExists()
        {
            // Arrange
            var room = new RoomEntity { Name = "Boardroom of Doom", Capacity = 12 };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            // Act
            var result = await _roomRepository.GetRoomByIdAsync(room.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Boardroom of Doom", result.Name);
            Assert.AreEqual(12, result.Capacity);
        }

        [TestMethod]
        public async Task GetRoomByIdAsync_ReturnsNull_WhenRoomDoesNotExist()
        {
            // Act
            var result = await _roomRepository.GetRoomByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion
    }
}
