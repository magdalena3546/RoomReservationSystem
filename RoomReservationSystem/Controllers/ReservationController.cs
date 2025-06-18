using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public ActionResult<IEnumerable<Reservation>> GetReservations()
        {
            return Ok(_context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .ToList());
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public ActionResult<Reservation> CreateReservation(Reservation reservation)
        {

            var room = _context.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
            if (room == null)
                return BadRequest("Invalid RoomId.");

            var guest = _context.Guests.FirstOrDefault(g => g.Id == reservation.GuestId);
            if (guest == null)
                return BadRequest("Invalid GuestId.");

            var isConflict = _context.Reservations.Any(r =>
                r.RoomId == reservation.RoomId &&
                r.StartTime < reservation.EndTime &&
                reservation.StartTime < r.EndTime);

            if (isConflict)
            {
                return Conflict("Room is already reserved during this time.");
            }

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }
    }
}
