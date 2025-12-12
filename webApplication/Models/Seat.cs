namespace webApplication.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }    // 1, 2, 3, ... (20 masa)

        // Available, InUse, Broken
        public string Status { get; set; } = "Available";

        // FK
        public int HallId { get; set; }
        public Hall Hall { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
