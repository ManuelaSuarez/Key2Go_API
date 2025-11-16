using Application.Abstraction;
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

        public PaymentService(IPaymentRepository paymentRepository, ITripRepository tripRepository, ICarRepository carRepository)
        {
            _paymentRepository = paymentRepository;
            _tripRepository = tripRepository;
            _carRepository = carRepository;
        }

        public async Task<List<PaymentResponse>> GetAll()
        {
            var response = await _paymentRepository.GetAllAsync();
            var listPayments = response
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    PaymentId = p.PaymentId,
                    PaymentDate = p.PaymentDate,
                    TotalAmount = p.TotalAmount,
                    Method = (int)p.Method,
                })
                .ToList();
            return listPayments;
        }

        public async Task<PaymentResponse?> GetById(int id)
        {
            var response = await _paymentRepository.GetByIdAsync(id) is Payment payment ?
                    new PaymentResponse()
                    {
                        Id = payment.Id,
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        TotalAmount = payment.TotalAmount,
                        Method = (int)payment.Method
                    } : null;

            return response;
        }

        public async Task<PaymentResponse?> GetByTripIdAsync(int tripId)
        {
            var payment = await _paymentRepository.GetByTripIdAsync(tripId);

            return payment == null ? null : new PaymentResponse
            {
                Id = payment.Id,
                PaymentId = payment.PaymentId,
                PaymentDate = payment.PaymentDate,
                TotalAmount = payment.TotalAmount,
                Method = (int)payment.Method
            };
        }

        // por que no devuelve <PaymentResponse?> NO DEBERÍA IR EN EL CONTROLLER PORQ NO QUEREMOS Q SE CREE UN PAGO DE LA NADA
        public async Task Create(int tripId, PaymentMethod method)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId)
                ?? throw new Exception("Trip not found.");

            if (!Enum.IsDefined(typeof(PaymentMethod), method))
                throw new Exception("Payment method not valid.");

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
                TotalAmount = payment.TotalAmount,
                Method = (int)payment.Method
            };
        }

        // EL UPDATE SI DEBERÍA IR EN EL CONTROLLER PERO SOLO PARA EDITAR EL METODO DE PAGO? YA QUE EL MONTO DEPENDE DE LOS DÍAS Y EL AUTO
        // QUE SE ACTUALIZA CUANDO SI SE ACTUALIZA EN LA RESERVA
        // Serían dos update distintos, uno que actualiza las cosas normales del payment y el metodo de pago y otro que actualiza el monto total por trip
        public async Task UpdateForTrip(int tripId, PaymentMethod method)
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
            payment.Method = method;

            await _paymentRepository.UpdateAsync(payment);
        }

        //public async Task<PaymentResponse?> Create(PaymentRequest request)
        //{
        //    var payment = new Payment
        //    {
        //        TotalAmount = request.TotalAmount,
        //        Method = (PaymentMethod)request.Method,
        //        TripId = request.TripId
        //    };

        //    payment = await _paymentRepository.CreateAsync(payment);

        //    return new PaymentResponse
        //    {
        //        Id = payment.Id,
        //        PaymentId = payment.PaymentId,
        //        PaymentDate = payment.PaymentDate,
        //        TotalAmount = payment.TotalAmount,
        //        Method = (int)payment.Method
        //    };
        //}

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
    }
}