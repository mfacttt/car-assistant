using CarAssistant.Data;
using CarAssistant.Features.CarProfile.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.CarProfile.Commands;

public record SaveCarCommand(
    int UserId,
    string Brand,
    string Model,
    string FuelType,
    string Vin,
    int Mileage,
    int Year,
    decimal EngineVolume);

public class SaveCarCommandHandler
{
    private readonly ApplicationDbContext _db;

    public SaveCarCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(SaveCarCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Cars.FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        if (existing is null)
        {
            var car = new Car
            {
                UserId = command.UserId,
                Brand = command.Brand.Trim(),
                Model = command.Model.Trim(),
                FuelType = command.FuelType.Trim(),
                Vin = command.Vin.Trim(),
                Mileage = command.Mileage,
                Year = command.Year,
                EngineVolume = command.EngineVolume
            };

            _db.Cars.Add(car);
        }
        else
        {
            existing.Brand = command.Brand.Trim();
            existing.Model = command.Model.Trim();
            existing.FuelType = command.FuelType.Trim();
            existing.Vin = command.Vin.Trim();
            existing.Mileage = command.Mileage;
            existing.Year = command.Year;
            existing.EngineVolume = command.EngineVolume;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
