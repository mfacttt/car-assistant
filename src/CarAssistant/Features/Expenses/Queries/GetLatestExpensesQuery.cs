using CarAssistant.Data;
using CarAssistant.Features.Expenses.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Expenses.Queries;

public record GetLatestExpensesQuery(int UserId, int Count);

public class GetLatestExpensesQueryHandler
{
    private readonly ApplicationDbContext _db;

    public GetLatestExpensesQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Expense>> Handle(GetLatestExpensesQuery query, CancellationToken cancellationToken = default)
    {
        return await _db.Expenses
            .Where(e => e.UserId == query.UserId)
            .OrderByDescending(e => e.OccurredAt)
            .Take(query.Count)
            .ToListAsync(cancellationToken);
    }
}
