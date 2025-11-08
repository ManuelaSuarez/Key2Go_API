using Contract.Trip.Request;
using Contract.Trip.Response;

namespace Application.Service
{
    public interface ITripService
    {
        Task<List<TripResponse>> GetAll();
        Task<TripResponse?> GetById(int id);
        Task<TripResponse?> Create(TripRequest request);
        Task<bool> Delete(int id);
        Task<TripResponse?> Update(int id, TripRequest request);
    }
}

