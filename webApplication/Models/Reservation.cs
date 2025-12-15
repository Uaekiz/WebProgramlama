using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class Reservation
    {
        public int Id { get; set; } //PK

        public int SeatId { get; set; } //FK
        public Seat Seat { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string UserId { get; set; }//FK

        [ForeignKey("UserId")]
        public User User { get; set; }//again ef
    }
}