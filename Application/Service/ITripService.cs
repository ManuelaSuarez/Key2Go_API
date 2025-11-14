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
        Task<TripResponse?> Update(int id, TripUpdate request);

        Task<bool> CancelTrip(int id);
        Task<bool> StartTrip(int id);
        Task<bool> FinishTrip(int id, int finalKm);
        Task<List<TripResponse>> GetByStatus(int status);
    }
}

