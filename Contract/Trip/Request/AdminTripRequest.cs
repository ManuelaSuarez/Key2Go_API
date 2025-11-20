using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Trip.Request
{
    public class AdminTripRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PaymentMethod { get; set; }

        // FK
        public int UserId { get; set; }
        public int CarId { get; set; }
    }
}