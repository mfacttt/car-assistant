namespace CarAssistant.Features.CarProfile.Models;

public class Car
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public int Year { get; set; }
    public decimal EngineVolume { get; set; }
}
