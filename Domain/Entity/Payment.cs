namespace Domain.Entity
{
    public class Payment : BaseEntity
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TotalAmount { get; set; }
        public PaymentMethod Method { get; set; }

        // FK
        public int TripId { get; set; }
        public Trip Trip { get; set; }
    }

    public enum PaymentMethod
    {
        Cash = 1,
        Card = 2,
        BankTransfer = 3
    }
}
