using CarAssistant.Data;
using CarAssistant.Features.Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dashboard.Queries;

public record GetNotificationsQuery(int UserId);

public class GetNotificationsQueryHandler
{
    private readonly ApplicationDbContext _db;

    public GetNotificationsQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Notification>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        return await _db.Notifications
            .Where(n => n.UserId == query.UserId)
            .OrderBy(n => n.Date)
            .ToListAsync(cancellationToken);
    }
}
