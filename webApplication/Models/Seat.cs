namespace webApplication.Models
{
    public class Seat
    {
        public int Id { get; set; } //This is PK

        public int SeatNumber { get; set; }

        public string Status { get; set; } = "Available";

        public int HallId { get; set; } //This is FK
        public Hall Hall { get; set; } //and again helping ef

        public List<Reservation> Reservations { get; set; } = new List<Reservation>(); //It saves the all old reservations
    }
}
