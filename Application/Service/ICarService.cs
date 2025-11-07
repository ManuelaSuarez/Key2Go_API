

using Contract.Car.Request;
using Contract.Car.Response;

namespace Application.Service
{
    public interface ICarService
    {
        Task<List<CarResponse>> GetAll();
        Task<CarResponse?> GetById(int id);
        Task<CarResponse?> Create(CarRequest request);
        Task<bool> Delete(int id);
        Task<CarResponse?> Update(int id, CarRequest request);
    }
}
