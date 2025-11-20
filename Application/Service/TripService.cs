using Application.Abstraction;
using Application.Service.Helpers.Validations;
using Contract.Trip.Request;
using Contract.Trip.Response;
using Domain.Entity;

namespace Application.Service
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ICarRepository _carRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICarService _carService;
        private readonly IPaymentService _paymentService;

        public TripService(ITripRepository tripRepository, ICarRepository carRepository, IUserRepository userRepository, ICarService carService, IPaymentService paymentService)
        {
            _tripRepository = tripRepository;
            _carRepository = carRepository;
            _userRepository = userRepository;
            _carService = carService;
            _paymentService = paymentService;
        }

        public async Task<List<TripResponse>> GetAllByUserId(int userId)
        {
            var response = await _tripRepository.GetByUserIdAsync(userId);
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
                    Status = (int)trip.Status,
                    UserId = trip.UserId,
                    CarId = trip.CarId
                })
                .ToList();
            return listTrips;
        }

        public async Task<TripResponse> GetByUserId(int tripId, int userId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.UserId != userId)
            {
                return null;
            }

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status,
                UserId = trip.UserId,
                CarId = trip.CarId
            };
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
                    Status = (int)trip.Status,
                    UserId = trip.UserId,
                    CarId = trip.CarId
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
                        Status = (int)trip.Status,
                        UserId = trip.UserId,
                        CarId = trip.CarId
                    } : null;

            return response;
        }

        public async Task<TripResponse?> AdminCreate(AdminTripRequest request)
        {

            if (!TripValidations.StartDateValidation(request.StartDate))
            {
                throw new Exception("Start date must be equal or greater than today.");
            }

            if (!TripValidations.EndDateValidation(request.StartDate, request.EndDate))
            {
                throw new Exception("End date must be equal or greater than start date.");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null || !user.IsActive)
            {
                throw new Exception("The selected user is inactive or does not exist");
            }

            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
            {
                throw new Exception("The car does not exist.");
            }

            bool isAvailable = await _carService.IsCarAvailable(
                request.CarId,
                request.StartDate,
                request.EndDate
            );

            if (!isAvailable)
                throw new Exception("The selected car is not available for the selected dates.");

            var trip = new Trip
            {
                ReservationNumber = GenerateReservationNumber(),
                CreationDate = DateTime.UtcNow,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = TripStatus.Pending,
                UserId = request.UserId,
                CarId = request.CarId
            };

            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
                throw new Exception("Payment method not valid.");

            trip = await _tripRepository.CreateAsync(trip);

            await _paymentService.Create(trip.Id, (PaymentMethod)request.PaymentMethod);

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status,
                UserId = trip.UserId,
                CarId = trip.CarId
            };
        }

        public async Task<TripResponse?> Create(int userId, TripRequest request)
        {

            if (!TripValidations.StartDateValidation(request.StartDate))
            {
                throw new Exception("Start date must be equal or greater than today.");
            }

            if (!TripValidations.EndDateValidation(request.StartDate, request.EndDate))
            {
                throw new Exception("End date must be equal or greater than start date.");
            }
                
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new Exception("The selected user is inactive or does not exist");
            }

            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
            {
                throw new Exception("The car does not exist.");
            }

            bool isAvailable = await _carService.IsCarAvailable(
                request.CarId,
                request.StartDate,
                request.EndDate
            );

            if (!isAvailable)
                throw new Exception("The selected car is not available for the selected dates.");

            var trip = new Trip
            {
                ReservationNumber = GenerateReservationNumber(),
                CreationDate = DateTime.UtcNow,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = TripStatus.Pending,
                UserId = userId,
                CarId = request.CarId
            };

            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
                throw new Exception("Payment method not valid.");

            trip = await _tripRepository.CreateAsync(trip);

            await _paymentService.Create(trip.Id, (PaymentMethod)request.PaymentMethod);

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status,
                UserId = trip.UserId,
                CarId = trip.CarId
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

        public async Task<TripResponse?> AdminUpdate(int id, AdminTripUpdate request)
        {
            var trip = await _tripRepository.GetByIdAsync(id);

            if (trip == null)
            {
                return null;
            }

            var tripStatus = (int)trip.Status;

            if (tripStatus != 1)
            {
                throw new Exception("You can only edit pending trips.");
            }

            if (!TripValidations.StartDateValidation(request.StartDate))
            {
                throw new Exception("Start date must be equal or greater than today.");
            }

            if (!TripValidations.EndDateValidation(request.StartDate, request.EndDate))
            {
                throw new Exception("End date must be equal or greater than start date.");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null || !user.IsActive)
            {
                throw new Exception("The selected user is inactive or does not exist");
            }

            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
            {
                throw new Exception("The car does not exist.");
            }

            bool datesChanged =
                trip.StartDate != request.StartDate ||
                trip.EndDate != request.EndDate;

            bool carChanged = trip.CarId != request.CarId;

            if (datesChanged || carChanged)
            {
                bool available = await _carService.IsCarAvailable(
                    request.CarId,
                    request.StartDate,
                    request.EndDate,
                    currentTripId: trip.Id
                );

                if (!available)
                    throw new Exception("The selected car is not available for the selected dates.");
            }

            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.UserId = request.UserId;
            trip.CarId = request.CarId;
                
            await _tripRepository.UpdateAsync(trip);

            if (datesChanged || carChanged)
            {
                await _paymentService.UpdateForTrip(trip.Id);
            }

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status,
                UserId = trip.UserId,
                CarId = trip.CarId
            };
        }

        public async Task<TripResponse?> Update(int id, int userId, TripUpdate request)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new Exception("The selected user is inactive or does not exist");
            }

            var tripStatus = (int)trip.Status;
            if (tripStatus != 1)
            {
                throw new Exception("You can only edit pending trips.");
            }

            if (!TripValidations.StartDateValidation(request.StartDate))
            {
                throw new Exception("Start date must be equal or greater than today.");
            }

            if (!TripValidations.EndDateValidation(request.StartDate, request.EndDate))
            {
                throw new Exception("End date must be equal or greater than start date.");
            }

            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
            {
                throw new Exception("The car does not exist.");
            }

            bool datesChanged =
                trip.StartDate != request.StartDate ||
                trip.EndDate != request.EndDate;

            bool carChanged = trip.CarId != request.CarId;

            if (datesChanged || carChanged)
            {
                bool available = await _carService.IsCarAvailable(
                    request.CarId,
                    request.StartDate,
                    request.EndDate,
                    currentTripId: trip.Id
                );

                if (!available)
                    throw new Exception("The selected car is not available for the selected dates.");
            }

            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.CarId = request.CarId;

            await _tripRepository.UpdateAsync(trip);

            if (datesChanged || carChanged)
            {
                await _paymentService.UpdateForTrip(trip.Id);
            }

            return new TripResponse
            {
                Id = trip.Id,
                ReservationNumber = trip.ReservationNumber,
                CreationDate = trip.CreationDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                InitialKm = trip.InitialKm,
                FinalKm = trip.FinalKm,
                Status = (int)trip.Status,
                UserId = trip.UserId,
                CarId = trip.CarId
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

            if (!TripValidations.FinalKmValidation(trip.InitialKm, finalKm))
            {
                throw new Exception("Final KM must be greater than Initial KM.");
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
                    Status = (int)t.Status,
                    UserId = t.UserId,
                    CarId = t.CarId
                })
                .ToList();
            return listTrips;
        }

        private static int GenerateReservationNumber()
        {
            return new Random().Next(100000, 999999);
        }
    }
}