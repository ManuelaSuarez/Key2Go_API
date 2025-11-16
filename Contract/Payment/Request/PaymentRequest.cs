using Domain.Entity;

namespace Contract.Payment.Request
{
    public class PaymentRequest
    {
        public int PaymentId { get; set; }
        public int Method { get; set; }
    }
}
