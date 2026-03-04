using System.ComponentModel.DataAnnotations;

namespace CarAssistant.Features.Expenses.Models;

public class ExpenseInputModel
{
    [Required]
    public string Type { get; set; } = string.Empty;

    public string? ServiceKind { get; set; }
    public string? Title { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    public decimal? Liters { get; set; }
    public decimal? PricePerLiter { get; set; }
    public int? Mileage { get; set; }
    public string? FuelType { get; set; }
    public string? Comment { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan Time { get; set; }
}
