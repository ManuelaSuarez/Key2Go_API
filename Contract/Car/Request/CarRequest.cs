
namespace Contract.Car.Request
{
    public class CarRequest
    {
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfManufacture { get; set; }
        public string Km { get; set; }
        public int DailyPrice { get; set; }
        public int Status { get; set; }
    }
}
