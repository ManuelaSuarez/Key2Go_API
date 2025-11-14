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
        private readonly ICarRepository _carRepository;

        public TripService(ITripRepository tripRepository, ICarRepository carRepository)
        {
            _tripRepository = tripRepository;
            _carRepository = carRepository;
        }

        public async Task<List<TripResponse>> GetAll()
        {
            var response = await _tripRepository.GetAllAsync();
            var listTrips = response
                .Select(trip => new TripResponse
                {
                    Id = trip.Id,
                    ReservationNumber = trip.ReservationNumber,
                    CreationDate = trip.CreationDate,
                    StartDate = trip.StartDate,
                    EndDate = trip.EndDate,
                    InitialKm = trip.InitialKm,
                    FinalKm = trip.FinalKm,
                    Status = (int)trip.Status
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
                        InitialKm = trip.InitialKm,
                        FinalKm = trip.FinalKm,
                        Status = (int)trip.Status
                    } : null;

            return response;
        }

        public async Task<TripResponse?> Create(TripRequest request)
        {
            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
                throw new Exception("El auto no existe");

            car.Status = CarStatus.Reserved;                  // ver si modificar la disponibilidad del auto en la creacion o al iniciar el trip
            await _carRepository.UpdateAsync(car);

            var trip = new Trip
            {
                ReservationNumber = request.ReservationNumber,
                CreationDate = DateTime.UtcNow,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
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
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status
            };
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _tripRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            await _tripRepository.DeleteAsync(entity);

            return true;
        }

        // a revisar si conviene manejarlo con un requestUpdate o flotarlo
        public async Task<TripResponse?> Update(int id, TripUpdate request)
        {
            var trip = await _tripRepository.GetByIdAsync(id);

            if (trip == null)
            {
                return null;
            }

            trip.ReservationNumber = request.ReservationNumber;
            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.InitialKm = request.FinalKm;
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
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status
            };
        }

        public async Task<bool> CancelTrip(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);

            if (trip == null)
            {
                return false;
            }

            var tripStatus = (int)trip.Status;

            if (tripStatus == 1)
            {
                trip.Status = TripStatus.Cancelled;
                await _tripRepository.UpdateAsync(trip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> StartTrip(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
            {
                return false;
            }

            var car = await _carRepository.GetByIdAsync(trip.CarId);
            trip.InitialKm = car.Km;

            trip.Status = TripStatus.Active;

            car.Status = CarStatus.InUse;

            await _tripRepository.UpdateAsync(trip);
            await _carRepository.UpdateAsync(car);

            return true;
        }
        public async Task<bool> FinishTrip(int id, int finalKm)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
            {
                return false;
            }

            trip.FinalKm = finalKm;
            trip.Status = TripStatus.Finished;

            var car = await _carRepository.GetByIdAsync(trip.CarId);
            car.Status = CarStatus.Available;
            car.Km = finalKm;

            await _tripRepository.UpdateAsync(trip);
            await _carRepository.UpdateAsync(car);

            return true;
        }

    public async Task<List<TripResponse>> GetByStatus(int status)
        {
            var response = await _tripRepository.GetByStatusAsync(status);
            var listTrips = response
                .Select(t => new TripResponse
                {
                    Id = t.Id,
                    ReservationNumber = t.ReservationNumber,
                    CreationDate = t.CreationDate,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    InitialKm = t.InitialKm,
                    FinalKm = t.FinalKm,
                    Status = (int)t.Status
                })
                .ToList();
            return listTrips;
        }
    }
}
