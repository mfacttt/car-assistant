using CarAssistant.Data;
using CarAssistant.Features.CarProfile.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.CarProfile.Queries;

public record GetUserCarQuery(int UserId);

public class GetUserCarQueryHandler
{
    private readonly ApplicationDbContext _db;

    public GetUserCarQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Car?> Handle(GetUserCarQuery query, CancellationToken cancellationToken = default)
    {
        return await _db.Cars.FirstOrDefaultAsync(c => c.UserId == query.UserId, cancellationToken);
    }
}
