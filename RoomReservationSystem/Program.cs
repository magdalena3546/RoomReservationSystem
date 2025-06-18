using RoomReservationSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RoomReservationSystem.Models;
using RoomReservationSystem.Services;
using SoapCore;

namespace RoomReservationSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



            builder.Services.AddControllers()
                .AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var key = builder.Configuration["Jwt:Key"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            builder.Services.AddScoped<IRoomAvailabilityService, RoomAvailabilityService>();

            builder.Services.AddRazorPages();
       

            builder.Services.AddHttpClient();
            builder.Services.AddSession();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseMiddleware<RoomReservationSystem.Middleware.LogHeadersMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

     
            app.UseSession();

  

            app.MapControllers();
            app.UseSoapEndpoint<IRoomAvailabilityService>("/RoomAvailabilityService.svc", new SoapEncoderOptions());
            app.MapRazorPages();

     

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!db.Guests.Any())
                {
                    db.Guests.Add(new Guest { Name = "John Doe", Email = "john@example.com" });
                    db.Guests.Add(new Guest { Name = "Jane Smith", Email = "jane@example.com" });
                }

                if (!db.Rooms.Any())
                {
                    db.Rooms.Add(new Room { Name = "Room A", Capacity = 10 });
                    db.Rooms.Add(new Room { Name = "Room B", Capacity = 20 });
                }

                if (!db.Users.Any())
                {
                    db.Users.Add(new User
                    {
                        Email = "admin@example.com",
                        PasswordHash = "admin123",
                        Role = "Admin"
                    });

                    db.Users.Add(new User
                    {
                        Email = "employee@example.com",
                        PasswordHash = "employee123",
                        Role = "Employee"
                    });
                }

                db.SaveChanges();
            }

            app.Run();
        }
    }
}
