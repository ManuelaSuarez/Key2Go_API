using Contract.Trip.Request;
using Contract.Trip.Response;

namespace Application.Service
{
    public interface ITripService
    {
        Task<List<TripResponse>> GetAllByUserId(int userId);
        Task<TripResponse> GetByUserId(int tripId, int userId);

        Task<List<TripResponse>> GetAll();
        Task<TripResponse?> GetById(int id);
        Task<TripResponse?> AdminCreate(AdminTripRequest request);
        Task<TripResponse?> Create(int userId, TripRequest request);
        Task<bool> Delete(int id);
        Task<TripResponse?> AdminUpdate(int id, AdminTripUpdate request);
        Task<TripResponse?> Update(int id, int userId, TripUpdate request);
        Task<bool> CancelTrip(int id);
        Task<bool> StartTrip(int id);
        Task<bool> FinishTrip(int id, int finalKm);
        Task<List<TripResponse>> GetByStatus(int status);
    }
}

