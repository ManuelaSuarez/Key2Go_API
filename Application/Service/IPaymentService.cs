using Contract.Payment.Request;
using Contract.Payment.Response;
using Domain.Entity;

namespace Application.Service
{
    public interface IPaymentService
    {
        Task<List<PaymentResponse>> GetAll();
        Task<PaymentResponse?> GetById(int id);
        Task<PaymentResponse?> GetByTripIdAsync(int tripId);
        Task Create(int tripId, PaymentMethod method);
        Task<PaymentResponse?> Update(int id, PaymentRequest request);
        Task UpdateForTrip(int tripId);
        //Task<PaymentResponse?> Create(PaymentRequest request);
        Task<bool> Delete(int id);
    }
}
