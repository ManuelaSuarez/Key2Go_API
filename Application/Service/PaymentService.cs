using Application.Abstraction;
using Contract.Payment.Request;
using Contract.Payment.Response;
using Domain.Entity;

namespace Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
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

        public async Task<PaymentResponse?> Create(PaymentRequest request)
        {
            var payment = new Payment
            {
                TotalAmount = request.TotalAmount,
                Method = (PaymentMethod)request.Method,
                TripId = request.TripId
            };

            payment = await _paymentRepository.CreateAsync(payment);

            return new PaymentResponse
            {
                Id = payment.Id,
                PaymentId = payment.PaymentId,
                PaymentDate = payment.PaymentDate,
                TotalAmount = payment.TotalAmount,
                Method = (int)payment.Method
            };
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

        public async Task<PaymentResponse?> Update(int id, PaymentRequest request)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            payment.TotalAmount = request.TotalAmount;
            payment.Method = (PaymentMethod)request.Method;
            payment.TripId = request.TripId;

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
    }
}