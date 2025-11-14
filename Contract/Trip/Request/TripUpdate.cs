
using Domain.Entity;

namespace Contract.Trip.Request
{
    public class TripUpdate
    {
        public int ReservationNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? InitialKm { get; set; }
        public int? FinalKm { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
    }
}
