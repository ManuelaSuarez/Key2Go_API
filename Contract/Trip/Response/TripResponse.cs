namespace Contract.Trip.Response
{
    public class TripResponse
    {
        public int Id { get; set; }
        public int ReservationNumber { get; set; }
        public DateTime CreationDate { get; set; } // Capaz se puede sacar
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
    }
}
