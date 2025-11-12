using Application.Abstraction;
using Application.Service;
using Contract.Trip.Request;
using Contract.Trip.Response;
using Domain.Entity;

namespace Application.Service
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;

        public TripService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public async Task<List<TripResponse>> GetAll()
        {
            // trips o response? en user es users y en car response...
            var response = await _tripRepository.GetAllAsync();
            var listTrips = response
                .Select(t => new TripResponse
                {
                    Id = t.Id,
                    ReservationNumber = t.ReservationNumber,
                    CreationDate = t.CreationDate,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Status = (int)t.Status
                })
                .ToList();
            return listTrips;
        }

        public async Task<TripResponse?> GetById(int id)
        {
            var response = await _tripRepository.GetByIdAsync(id) is Trip trip ?
                    new TripResponse()
                    {
                        Id = trip.Id,
                        ReservationNumber = trip.ReservationNumber,
                        CreationDate = trip.CreationDate,
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        Status = (int)trip.Status
                    } : null;

            return response;
        }

        public async Task<TripResponse?> Create(TripRequest request)
        {
            var trip = new Trip
            {
                ReservationNumber = request.ReservationNumber,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                FinalKm = request.FinalKm,
                Status = (TripStatus)request.Status,
                UserId = request.UserId,
                CarId = request.CarId
            };

            trip = await _tripRepository.CreateAsync(trip);

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Status = (int)trip.Status
            };
        }

        public async Task<bool> Delete(int id)
        {
            // por qué entity y no trip?
            var entity = await _tripRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            await _tripRepository.DeleteAsync(entity);

            return true;
        }

        public async Task<TripResponse?> Update(int id, TripRequest request)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            
            if (trip == null)
            {
                return null;
            }

            trip.ReservationNumber = request.ReservationNumber;
            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.FinalKm = request.FinalKm;
            trip.Status = (TripStatus)request.Status;
            trip.UserId = request.UserId;
            trip.CarId = request.CarId;

            await _tripRepository.UpdateAsync(trip);

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Status = (int)trip.Status
            };
        }
    }
}
