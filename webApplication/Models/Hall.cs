namespace webApplication.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public string Type { get; set; }       
        public int SeatCount { get; set; }    
        public string? Description { get; set; }


        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
