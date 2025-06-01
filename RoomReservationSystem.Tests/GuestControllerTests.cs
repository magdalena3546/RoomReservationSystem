using Xunit;
using Microsoft.EntityFrameworkCore;
using RoomReservationSystem.Controllers;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RoomReservationSystem.Tests.Controllers
{
    public class GuestControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GuestTestDb_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Guests.Add(new Guest { Id = 1, Name = "John Doe", Email = "john@example.com" });
            context.Guests.Add(new Guest { Id = 2, Name = "Jane Smith", Email = "jane@example.com" });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetGuests_ReturnsAllGuests()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var result = await controller.GetGuests();

            var okResult = Assert.IsType<ActionResult<IEnumerable<Guest>>>(result);
            var guests = Assert.IsType<List<Guest>>(okResult.Value);
            Assert.Equal(2, guests.Count);
        }

        [Fact]
        public async Task GetGuest_ReturnsGuest_WhenGuestExists()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var result = await controller.GetGuest(1);

            var actionResult = Assert.IsType<ActionResult<Guest>>(result);
            var guest = Assert.IsType<Guest>(actionResult.Value);
            Assert.Equal(1, guest.Id);
            Assert.Equal("John Doe", guest.Name);
        }

        [Fact]
        public async Task GetGuest_ReturnsNotFound_WhenGuestDoesNotExist()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var result = await controller.GetGuest(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateGuest_AddsGuestSuccessfully()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var newGuest = new Guest { Name = "New Guest", Email = "new@example.com" };

            var result = await controller.CreateGuest(newGuest);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdGuest = Assert.IsType<Guest>(createdAtActionResult.Value);
            Assert.Equal("New Guest", createdGuest.Name);
            Assert.Equal("new@example.com", createdGuest.Email);
        }

        [Fact]
        public async Task UpdateGuest_UpdatesSuccessfully()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var existingGuest = await context.Guests.FirstAsync();
            existingGuest.Name = "Updated Name";

            var result = await controller.UpdateGuest(existingGuest.Id, existingGuest);

            Assert.IsType<NoContentResult>(result);

            var updatedGuest = await context.Guests.FindAsync(existingGuest.Id);
            Assert.Equal("Updated Name", updatedGuest.Name);
        }

        [Fact]
        public async Task DeleteGuest_DeletesSuccessfully()
        {
            var context = GetDbContext();
            var controller = new GuestController(context);

            var result = await controller.DeleteGuest(1);

            Assert.IsType<NoContentResult>(result);

            var deletedGuest = await context.Guests.FindAsync(1);
            Assert.Null(deletedGuest);
        }
    }
}
