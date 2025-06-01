using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomReservationSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }

     
        [Required]
        public int RoomId { get; set; }

        
        public Room? Room { get; set; }


        [Required]
        public int GuestId { get; set; }

  
        public Guest? Guest { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
