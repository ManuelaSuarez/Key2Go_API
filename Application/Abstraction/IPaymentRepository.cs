using Domain.Entity;

namespace Application.Abstraction
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<Payment?> GetByTripIdAsync(int tripId);
    }
}