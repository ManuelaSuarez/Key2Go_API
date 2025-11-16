using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Contract.Payment.Request;
using Contract.Payment.Response;
using Domain.Entity;

namespace Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ICarRepository _carRepository;
        private readonly IUsdArsRateService _usdArsRateService;

        public PaymentService(IPaymentRepository paymentRepository, ITripRepository tripRepository, ICarRepository carRepository, IUsdArsRateService usdArsRateService)
        {
            _paymentRepository = paymentRepository;
            _tripRepository = tripRepository;
            _carRepository = carRepository;
            _usdArsRateService = usdArsRateService;
        }

        public async Task<List<PaymentResponse>> GetAll()
        {
            var response = await _paymentRepository.GetAllAsync();

            var usdRate = await _usdArsRateService.GetUsdArsRateAsync();

            var listPayments = response
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    PaymentId = p.PaymentId,
                    PaymentDate = p.PaymentDate,
                    TotalAmountUsd = p.TotalAmount,
                    TotalAmountArs = usdRate.HasValue ? CalculateArs(p.TotalAmount, usdRate.Value) : 0,
                    Method = (int)p.Method,
                })
                .ToList();
            return listPayments;
        }

        public async Task<PaymentResponse?> GetById(int id)
        {
            var usdRate = await _usdArsRateService.GetUsdArsRateAsync();
            
            var response = await _paymentRepository.GetByIdAsync(id) is Payment payment ?
                    new PaymentResponse()
                    {
                        Id = payment.Id,
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        TotalAmountUsd = payment.TotalAmount,
                        TotalAmountArs = usdRate.HasValue ? CalculateArs(payment.TotalAmount, usdRate.Value) : 0,
                        Method = (int)payment.Method
                    } : null;

            return response;
        }

        public async Task<PaymentResponse?> GetByTripIdAsync(int tripId)
        {
            var usdRate = await _usdArsRateService.GetUsdArsRateAsync();

            var payment = await _paymentRepository.GetByTripIdAsync(tripId);

            return payment == null ? null : new PaymentResponse
            {
                Id = payment.Id,
                PaymentId = payment.PaymentId,
                PaymentDate = payment.PaymentDate,
                TotalAmountUsd = payment.TotalAmount,
                TotalAmountArs = usdRate.HasValue ? CalculateArs(payment.TotalAmount, usdRate.Value) : 0,
                Method = (int)payment.Method
            };
        }

        public async Task Create(int tripId, PaymentMethod method)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId)
                ?? throw new Exception("Trip not found.");

            var existingPayment = await _paymentRepository.GetByTripIdAsync(trip.Id);
            if (existingPayment != null)
                throw new Exception("The trip already has a payment.");

            var car = await _carRepository.GetByIdAsync(trip.CarId)
                ?? throw new Exception("Car not found.");

            var amount = CalculateAmount(trip, car);

            var payment = new Payment
            {
                PaymentDate = DateTime.UtcNow,
                TotalAmount = amount,
                Method = method,
                TripId = trip.Id
            };

            await _paymentRepository.CreateAsync(payment);
        }

        public async Task<PaymentResponse?> Update(int id, PaymentRequest request)
        {
            var usdRate = await _usdArsRateService.GetUsdArsRateAsync();

            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            payment.Method = (PaymentMethod)request.Method;

            await _paymentRepository.UpdateAsync(payment);

            return new PaymentResponse
            {
                Id = payment.Id,
                PaymentId = payment.PaymentId,
                PaymentDate = payment.PaymentDate,
                TotalAmountUsd = payment.TotalAmount,
                TotalAmountArs = usdRate.HasValue ? CalculateArs(payment.TotalAmount, usdRate.Value) : 0,
                Method = (int)payment.Method
            };
        }

        public async Task UpdateForTrip(int tripId)
        {

            var trip = await _tripRepository.GetByIdAsync(tripId)
                ?? throw new Exception("Trip not found");

            var payment = await _paymentRepository.GetByTripIdAsync(trip.Id)
                ?? throw new Exception("Payment not found");

            var car = await _carRepository.GetByIdAsync(trip.CarId)
                ?? throw new Exception("Car not found");

            var amount = CalculateAmount(trip, car);

            payment.PaymentDate = DateTime.UtcNow;
            payment.TotalAmount = CalculateAmount(trip, car);

            await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<bool> Delete(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return false;
            }

            await _paymentRepository.DeleteAsync(payment);

            return true;
        }

        private decimal CalculateAmount(Trip trip, Car car)
        {
            var tripDays = (trip.EndDate - trip.StartDate).Days;

            if (tripDays <= 0)
            {
                tripDays = 1;
            }

            return tripDays * car.DailyPriceUsd;
        }

        private decimal CalculateArs(decimal amountUsd, decimal usdRate)
        {
            return amountUsd * usdRate;
        }
    }
}