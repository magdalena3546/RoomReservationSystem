using Xunit;
using Microsoft.EntityFrameworkCore;
using RoomReservationSystem.Controllers;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomReservationSystem.Tests.Controllers
{
    public class RoomControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "RoomTestDb_" + Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Rooms.Add(new Room { Id = 1, Name = "Room A", Capacity = 10 });
            context.Rooms.Add(new Room { Id = 2, Name = "Room B", Capacity = 20 });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetRooms_ReturnsAllRooms()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var result = await controller.GetRooms();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Room>>>(result);
            var rooms = Assert.IsType<List<Room>>(actionResult.Value);
            Assert.Equal(2, rooms.Count);
        }

        [Fact]
        public async Task GetRoom_ReturnsRoom_WhenExists()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var result = await controller.GetRoom(1);

            var actionResult = Assert.IsType<ActionResult<Room>>(result);
            var room = Assert.IsType<Room>(actionResult.Value);
            Assert.Equal(1, room.Id);
            Assert.Equal("Room A", room.Name);
        }

        [Fact]
        public async Task GetRoom_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var result = await controller.GetRoom(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateRoom_AddsRoomSuccessfully()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var newRoom = new Room { Name = "New Room", Capacity = 15 };

            var result = await controller.CreateRoom(newRoom);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdRoom = Assert.IsType<Room>(createdAtResult.Value);
            Assert.Equal("New Room", createdRoom.Name);
            Assert.Equal(15, createdRoom.Capacity);
        }

        [Fact]
        public async Task UpdateRoom_UpdatesSuccessfully()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var existingRoom = await context.Rooms.FirstAsync();
            existingRoom.Name = "Updated Room";

            var result = await controller.UpdateRoom(existingRoom.Id, existingRoom);

            Assert.IsType<NoContentResult>(result);

            var updatedRoom = await context.Rooms.FindAsync(existingRoom.Id);
            Assert.Equal("Updated Room", updatedRoom.Name);
        }

        [Fact]
        public async Task DeleteRoom_DeletesSuccessfully()
        {
            var context = GetDbContext();
            var controller = new RoomController(context);

            var result = await controller.DeleteRoom(1);

            Assert.IsType<NoContentResult>(result);

            var deletedRoom = await context.Rooms.FindAsync(1);
            Assert.Null(deletedRoom);
        }
    }
}
