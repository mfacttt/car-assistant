using System.Collections.Generic;
using CarAssistant.Features.CarProfile.Models;
using CarAssistant.Features.Expenses.Models;

namespace CarAssistant.Features.Dashboard.Models;

public class HomeDashboardViewModel
{
    public Car? Car { get; set; }
    public IReadOnlyList<Notification> Notifications { get; set; } = new List<Notification>();
    public IReadOnlyList<Expense> LatestExpenses { get; set; } = new List<Expense>();
    public decimal? AverageConsumptionLPer100Km { get; set; }
    public int? TotalDistanceKm { get; set; }
}
