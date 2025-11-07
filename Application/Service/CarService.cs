

using Application.Abstraction;
using Contract.Car.Request;
using Contract.Car.Response;
using Domain.Entity;


namespace Application.Service
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }


        public async Task<List<CarResponse>> GetAll()
        {
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
                    DailyPrice = c.DailyPrice,
                    Status = (int)c.Status
                })
                .ToList();
            return listCars;
        }

        public async Task<CarResponse?> GetById(int id)
        {
            var response = await _carRepository.GetByIdAsync(id) is Car car ?
                    new CarResponse()
                    {
                        Id = car.Id,
                        LicensePlate = car.LicensePlate,
                        Brand = car.Brand,
                        Model = car.Model,
                        YearOfManufacture = car.YearOfManufacture,
                        Km = car.Km,
                        DailyPrice = car.DailyPrice,
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
                DailyPrice = request.DailyPrice,
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
                DailyPrice = car.DailyPrice,
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
                return null;

            car.LicensePlate = request.LicensePlate;
            car.Brand = request.Brand;
            car.Model = request.Model;
            car.YearOfManufacture = request.YearOfManufacture;
            car.Km = request.Km;
            car.DailyPrice = request.DailyPrice;
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
                DailyPrice = car.DailyPrice,
                Status = (int)car.Status
            };
        }
    }
}
