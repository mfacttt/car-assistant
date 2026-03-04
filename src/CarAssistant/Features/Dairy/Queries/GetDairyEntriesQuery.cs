using CarAssistant.Data;
using CarAssistant.Features.Dairy.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dairy.Queries;

public record GetDairyEntriesQuery(int UserId);

public class GetDairyEntriesQueryHandler
{
    private readonly ApplicationDbContext _db;

    public GetDairyEntriesQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<DairyEntry>> Handle(GetDairyEntriesQuery query, CancellationToken cancellationToken = default)
    {
        return await _db.DairyEntries
            .Where(e => e.UserId == query.UserId)
            .Include(e => e.Photos)
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);
    }
}
