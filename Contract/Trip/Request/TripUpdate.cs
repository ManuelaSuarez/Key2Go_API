
using Domain.Entity;

namespace Contract.Trip.Request
{
    public class TripUpdate
    {
        public int ReservationNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
    }
}
