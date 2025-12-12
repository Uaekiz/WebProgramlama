using System;

namespace webApplication.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        // Which seat is reserved?
        public int SeatId { get; set; }
        public Seat Seat { get; set; }

        // Reservation time range
        public DateTime StartTime { get; set; }   // e.g. 2025-12-10 14:00
        public DateTime EndTime { get; set; }     // e.g. 2025-12-10 15:00

        // Who reserved this seat? (simple for now)
        public string UserName { get; set; }      // later you can change to UserId
    }
}
