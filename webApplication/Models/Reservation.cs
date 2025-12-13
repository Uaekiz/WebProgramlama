using System;
using System.ComponentModel.DataAnnotations.Schema; // Bunu eklemeyi unutma

namespace webApplication.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int SeatId { get; set; }
        public Seat Seat { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}