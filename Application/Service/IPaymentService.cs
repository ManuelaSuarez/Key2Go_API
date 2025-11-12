using Contract.Payment.Request;
using Contract.Payment.Response;

namespace Application.Service
{
    public interface IPaymentService
    {
        Task<List<PaymentResponse>> GetAll();
        Task<PaymentResponse?> GetById(int id);
        Task<PaymentResponse?> Create(PaymentRequest request);
        Task<bool> Delete(int id);
        Task<PaymentResponse?> Update(int id, PaymentRequest request);
    }
}
