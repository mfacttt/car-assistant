using CarAssistant.Data;
using CarAssistant.Features.Expenses.Models;

namespace CarAssistant.Features.Expenses.Commands;

public record SaveExpenseCommand(
    int UserId,
    ExpenseType Type,
    string? ServiceKind,
    string? Title,
    decimal Amount,
    decimal? Liters,
    decimal? PricePerLiter,
    int? Mileage,
    string? FuelType,
    string? Comment,
    DateTime OccurredAt);

public class SaveExpenseCommandHandler
{
    private readonly ApplicationDbContext _db;

    public SaveExpenseCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(SaveExpenseCommand command, CancellationToken cancellationToken = default)
    {
        var expense = new Expense
        {
            UserId = command.UserId,
            Type = command.Type,
            ServiceKind = string.IsNullOrWhiteSpace(command.ServiceKind) ? null : command.ServiceKind.Trim(),
            Title = string.IsNullOrWhiteSpace(command.Title) ? null : command.Title.Trim(),
            Amount = command.Amount,
            Liters = command.Liters,
            PricePerLiter = command.PricePerLiter,
            Mileage = command.Mileage,
            FuelType = string.IsNullOrWhiteSpace(command.FuelType) ? null : command.FuelType.Trim(),
            Comment = string.IsNullOrWhiteSpace(command.Comment) ? null : command.Comment.Trim(),
            OccurredAt = command.OccurredAt
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
