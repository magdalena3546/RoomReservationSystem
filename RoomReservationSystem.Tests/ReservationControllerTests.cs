using Xunit;
using Microsoft.EntityFrameworkCore;
using RoomReservationSystem.Controllers;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomReservationSystem.Tests.Controllers
{
    public class ReservationControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ReservationTestDb_" + Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Rooms.Add(new Room { Id = 1, Name = "Test Room", Capacity = 10 });
            context.Guests.Add(new Guest { Id = 1, Name = "Test Guest", Email = "test@example.com" });

            context.Reservations.Add(new Reservation
            {
                RoomId = 1,
                GuestId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetReservations_ReturnsOkWithList()
        {
            var context = GetDbContext();
            var controller = new ReservationController(context);

            var result = controller.GetReservations();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reservations = Assert.IsAssignableFrom<IEnumerable<Reservation>>(okResult.Value);
            Assert.NotEmpty(reservations);
        }

        [Fact]
        public void CreateReservation_ValidData_ReturnsCreated()
        {
            var context = GetDbContext();
            var controller = new ReservationController(context);

            var newReservation = new Reservation
            {
                RoomId = 1,
                GuestId = 1,
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            };

            var result = controller.CreateReservation(newReservation);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var reservation = Assert.IsType<Reservation>(createdResult.Value);
            Assert.Equal(newReservation.RoomId, reservation.RoomId);
            Assert.Equal(newReservation.GuestId, reservation.GuestId);
        }

        [Fact]
        public void CreateReservation_InvalidRoomId_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new ReservationController(context);

            var newReservation = new Reservation
            {
                RoomId = 999,
                GuestId = 1,
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            };

            var result = controller.CreateReservation(newReservation);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid RoomId.", badRequest.Value);
        }

        [Fact]
        public void CreateReservation_InvalidGuestId_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new ReservationController(context);

            var newReservation = new Reservation
            {
                RoomId = 1,
                GuestId = 999,
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            };

            var result = controller.CreateReservation(newReservation);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid GuestId.", badRequest.Value);
        }

        [Fact]
        public void CreateReservation_Conflict_ReturnsConflict()
        {
            var context = GetDbContext();
            var controller = new ReservationController(context);

            var newReservation = new Reservation
            {
                RoomId = 1,
                GuestId = 1,
                StartTime = DateTime.UtcNow.AddHours(1).AddMinutes(30),
                EndTime = DateTime.UtcNow.AddHours(2).AddMinutes(30)
            };

            var result = controller.CreateReservation(newReservation);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Room is already reserved during this time.", conflictResult.Value);
        }
    }
}
