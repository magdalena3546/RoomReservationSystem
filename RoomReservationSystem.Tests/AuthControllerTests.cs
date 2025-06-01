using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoomReservationSystem.Controllers;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RoomReservationSystem.Tests.Controllers
{
    public class AuthControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            
            context.Users.Add(new User
            {
                Email = "admin@example.com",
                PasswordHash = "admin123",
                Role = "Admin"
            });

            context.SaveChanges();
            return context;
        }

        private IConfiguration GetConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "super_secret_key_1234567890_super_secret_key"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
   
            var dbContext = GetDbContext();
            var configuration = GetConfiguration();
            var controller = new AuthController(dbContext, configuration);

            var loginRequest = new RoomReservationSystem.Controllers.UserLogin
            {
                Email = "admin@example.com",
                Password = "admin123"
            };

           
            var result = controller.Login(loginRequest);

           
            var okResult = Assert.IsType<OkObjectResult>(result);

          
            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            var responseDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.NotNull(responseDict);
            Assert.True(responseDict.ContainsKey("token"));
            Assert.False(string.IsNullOrWhiteSpace(responseDict["token"]));
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
           
            var dbContext = GetDbContext();
            var configuration = GetConfiguration();
            var controller = new AuthController(dbContext, configuration);

            var loginRequest = new RoomReservationSystem.Controllers.UserLogin
            {
                Email = "wrong@example.com",
                Password = "wrongpassword"
            };

          
            var result = controller.Login(loginRequest);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
