using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Application.Service.Helpers.Normalitazion;
using Application.Service.Helpers.Validations;
using Contract.Car.Request;
using Contract.Car.Response;
using Domain.Entity;

namespace Application.Service
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUsdArsRateService _usdArsRateService;
        public CarService(ICarRepository carRepository, ITripRepository tripRepository, IUsdArsRateService usdArsRateService)
        {
            _carRepository = carRepository;
            _tripRepository = tripRepository;
            _usdArsRateService = usdArsRateService;
        }


        public async Task<List<CarResponse>> GetAll()
        {
            var rate = await _usdArsRateService.GetUsdArsRateAsync();
            if (rate == null)
            {
                rate=0;
            }

            var response = await _carRepository.GetAllAsync();
            var listCars = response
                .Select(c => new CarResponse
                {
                    Id = c.Id,
                    LicensePlate = c.LicensePlate,
                    Brand = c.Brand,
                    Model = c.Model,
                    YearOfManufacture = c.YearOfManufacture,
                    Km = c.Km,
                    DailyPriceUsd = c.DailyPriceUsd,
                    DailyPriceArs = c.DailyPriceUsd * rate.Value,
                    Status = (int)c.Status
                })
                .ToList();
            return listCars;
        }

        public async Task<CarResponse?> GetById(int id)
        {
            var rate = await _usdArsRateService.GetUsdArsRateAsync();
            if (rate == null)
            {
                rate = 0;
            }

            var response = await _carRepository.GetByIdAsync(id) is Car car ?
                    new CarResponse()
                    {
                        Id = car.Id,
                        LicensePlate = car.LicensePlate,
                        Brand = car.Brand,
                        Model = car.Model,
                        YearOfManufacture = car.YearOfManufacture,
                        Km = car.Km,
                        DailyPriceUsd = car.DailyPriceUsd,
                        DailyPriceArs = car.DailyPriceUsd * rate.Value,
                        Status = (int)car.Status
                    } : null;

            return response;


        }

        public async Task<CarResponse?> Create(CarRequest request)
        {
            var existingLicensePlate = await _carRepository.GetByLicensePlate(request.LicensePlate);

            if (existingLicensePlate != null)
            {
                throw new Exception($"Car with license plate {request.LicensePlate} already exists");
            }

            if (!(CarValidations.LicensePlateValidation(request.LicensePlate)))
            {
                throw new Exception("invalid license plate format");
            } 
            if(!(CarValidations.YearOfManufactureValidation(request.YearOfManufacture)))
            {
                var currentYear= DateTime.UtcNow.Year;
                throw new Exception($"Year of manufacture must be between 1900 and {currentYear}");
            }
            if (!(CarValidations.KmValidation(request.Km)))
            {
                throw new Exception("Car Km cannot be less than 0");
            }
            if(!(CarValidations.DailyPriceValidation(request.DailyPriceUsd)))
            {
                throw new Exception("Price must be greater than 0");
            }
            if(!(CarValidations.StatusValidation(request.Status)))
            {
                throw new Exception("Status can only be 1 or 2");
            }

            var car = new Car             
            {
                LicensePlate = TextNormalization.Normalization(request.LicensePlate),
                Brand = TextNormalization.Normalization(request.Brand),
                Model = TextNormalization.Normalization(request.Model),
                YearOfManufacture = request.YearOfManufacture,
                Km = request.Km,
                DailyPriceUsd = request.DailyPriceUsd,
                Status = (CarStatus)request.Status
            };

            car = await _carRepository.CreateAsync(car);

            return new CarResponse
            {
                Id = car.Id,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                YearOfManufacture = car.YearOfManufacture,
                Km = car.Km,
                DailyPriceUsd = car.DailyPriceUsd,
                Status = (int)car.Status
            };
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _carRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            await _carRepository.DeleteAsync(entity);

            return true;
        }

        public async Task<CarResponse?> Update(int id, CarRequest request)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return null;
            }

            var existingLicensePlate = await _carRepository.GetByLicensePlate(request.LicensePlate);
            if (existingLicensePlate != null && existingLicensePlate.Id != id)
            {
                throw new Exception($"Car with license plate {request.LicensePlate} already exists");
            }


            if (!(CarValidations.LicensePlateValidation(request.LicensePlate)))
            {
                throw new Exception("invalid License Plate format");
            }
            if (!(CarValidations.YearOfManufactureValidation(request.YearOfManufacture)))
            {
                var currentYear = DateTime.UtcNow.Year;
                throw new Exception($"Year of manufacture must be between 1900 and {currentYear}");
            }
            if (!(CarValidations.KmValidation(request.Km)))
            {
                throw new Exception("Car Km cannot be less than 0");
            }
            if (!(CarValidations.DailyPriceValidation(request.DailyPriceUsd)))
            {
                throw new Exception("Price must be greater than 0");
            }
            if (!(CarValidations.StatusValidation(request.Status)))
            {
                throw new Exception("Status can only be 1 or 2");
            }

            car.LicensePlate = TextNormalization.Normalization(request.LicensePlate);
            car.Brand = TextNormalization.Normalization(request.Brand);
            car.Model = TextNormalization.Normalization(request.Model);
            car.YearOfManufacture = request.YearOfManufacture;
            car.Km = request.Km;
            car.DailyPriceUsd = request.DailyPriceUsd;
            car.Status = (CarStatus)request.Status;

            await _carRepository.UpdateAsync(car);

            return new CarResponse
            {
                Id = car.Id,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                YearOfManufacture = car.YearOfManufacture,
                Km = car.Km,
                DailyPriceUsd = car.DailyPriceUsd,
                Status = (int)car.Status
            };
        }
    
        public async Task<bool> IsCarAvailable(int id, DateTime startDate, DateTime endDate, int? currentTripId = null)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return false;

            var trips = await _tripRepository.GetByCarIdAsync(id);

            foreach (var trip in trips)
            {
                if (currentTripId.HasValue && trip.Id == currentTripId.Value)
                    continue; // Ignorar la reserva que estamos modificando

                if (trip.Status == TripStatus.Cancelled)
                    continue; // Ignorar las reservas canceladas

                bool overlap = startDate < trip.EndDate && endDate > trip.StartDate;
                
                if (overlap)
                        return false;
            }

            return true;
        }
    }
}