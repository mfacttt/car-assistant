namespace CarAssistant.Features.Expenses.Models;

public enum ExpenseType
{
    Service,
    Fuel,
    Other
}

public class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ExpenseType Type { get; set; }
    public string? ServiceKind { get; set; }
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public decimal? Liters { get; set; }
    public decimal? PricePerLiter { get; set; }
    public int? Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Comment { get; set; }
    public DateTime OccurredAt { get; set; }
}
