namespace Domain.Entity
{
    public class Trip : BaseEntity
    {
        public int ReservationNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? InitialKm { get; set; }
        public int? FinalKm { get; set; }
        public TripStatus Status { get; set; }
        public Payment Payment { get; set; } // Para que EF sepa que es 1:1

        //FK
        public int UserId { get; set; }
        public User User { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
    }

    public enum TripStatus
    {
        Pending = 1,
        Active = 2,
        Cancelled = 3,
        Finished = 4
    }
}
