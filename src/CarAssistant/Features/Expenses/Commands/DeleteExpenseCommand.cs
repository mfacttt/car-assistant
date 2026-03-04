using CarAssistant.Data;
using CarAssistant.Features.Expenses.Models;

namespace CarAssistant.Features.Expenses.Commands;

public record DeleteExpenseCommand(int Id, int UserId);

public class DeleteExpenseCommandHandler
{
    private readonly ApplicationDbContext _db;

    public DeleteExpenseCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteExpenseCommand command, CancellationToken cancellationToken = default)
    {
        var expense = await _db.Expenses.FindAsync(new object[] { command.Id }, cancellationToken);
        if (expense == null || expense.UserId != command.UserId)
        {
            return;
        }

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
