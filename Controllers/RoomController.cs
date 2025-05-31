using Microsoft.AspNetCore.Mvc;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            return Ok(_context.Rooms.ToList());
        }

        [HttpPost]
        public ActionResult<Room> CreateRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetRooms), new { id = room.Id }, room);
        }
    }
}

