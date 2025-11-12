using Domain.Entity;

namespace Contract.Payment.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TotalAmount { get; set; }
        public int Method { get; set; }
    }
}