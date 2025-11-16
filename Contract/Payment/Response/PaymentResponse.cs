using Domain.Entity;

namespace Contract.Payment.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmountUsd { get; set; }
        public decimal TotalAmountArs { get; set; }
        public int Method { get; set; }
    }
}