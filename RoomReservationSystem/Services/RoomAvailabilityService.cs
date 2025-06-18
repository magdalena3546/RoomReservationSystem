using System;
using System.Linq;
using RoomReservationSystem.Data;

namespace RoomReservationSystem.Services
{
    public class RoomAvailabilityService : IRoomAvailabilityService
    {
        private readonly AppDbContext _context;

        public RoomAvailabilityService(AppDbContext context)
        {
            _context = context;
        }

        public bool CheckRoomAvailability(int roomId, DateTime startTime, DateTime endTime)
        {
            var overlappingReservation = _context.Reservations
                .Where(r => r.RoomId == roomId)
                .Any(r =>
                    (startTime < r.EndTime) && (endTime > r.StartTime)
                );

            return !overlappingReservation;
        }
    }
}
