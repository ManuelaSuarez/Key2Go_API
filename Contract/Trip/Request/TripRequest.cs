namespace Contract.Trip.Request
{
    public class TripRequest
    {
        public int ReservationNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? FinalKm { get; set; }
        public int Status { get; set; }

        // FK
        public int UserId { get; set; }
        public int CarId { get; set; }
    }
}