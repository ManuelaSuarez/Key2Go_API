namespace Contract.Trip.Request
{
    public class TripRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PaymentMethod { get; set; }

        // FK
        public int CarId { get; set; }
    }
}