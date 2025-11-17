using Domain.Entity;

namespace Application.Abstraction
{
    public interface ITripRepository : IBaseRepository<Trip>
    {
        Task<List<Trip>> GetByStatusAsync(int status);
        Task<List<Trip>> GetByCarIdAsync(int carId);
    }
}