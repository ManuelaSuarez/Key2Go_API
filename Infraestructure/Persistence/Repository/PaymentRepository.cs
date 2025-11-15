using Application.Abstraction;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Persistence.Repository
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly Key2GoDbContext _context;

        public PaymentRepository(Key2GoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByTripIdAsync(int tripId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TripId == tripId);
        }
    }
}
