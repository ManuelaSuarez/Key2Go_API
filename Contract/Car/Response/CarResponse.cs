
namespace Contract.Car.Response
{
    public class CarResponse
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfManufacture { get; set; }
        public int Km { get; set; }
        public decimal DailyPriceUsd { get; set; }
        public decimal DailyPriceArs { get; set; }
        public int Status { get; set; }
    }
}
