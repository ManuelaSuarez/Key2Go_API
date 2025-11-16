using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Contract.Car.Request;
using Contract.Car.Response;
using Domain.Entity;

//Agregar validacion para ser si el CAR no tiene reservas dentro de una fecha especifica
//Revisar metodo para mantenimiento automatico

namespace Application.Service
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IUsdArsRateService _usdArsRateService;
        public CarService(ICarRepository carRepository, IUsdArsRateService usdArsRateService)
        {
            _carRepository = carRepository;
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
            var car = new Car             {
                LicensePlate = request.LicensePlate,
                Brand = request.Brand,
                Model = request.Model,
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

            car.LicensePlate = request.LicensePlate;
            car.Brand = request.Brand;
            car.Model = request.Model;
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
    }
}