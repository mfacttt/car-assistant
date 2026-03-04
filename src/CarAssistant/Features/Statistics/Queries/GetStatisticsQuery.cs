using System.Globalization;
using CarAssistant.Data;
using CarAssistant.Features.Expenses.Models;
using CarAssistant.Features.Statistics.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Statistics.Queries;

public record GetStatisticsQuery(int UserId);

public class GetStatisticsQueryHandler
{
    private readonly ApplicationDbContext _db;

    public GetStatisticsQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<StatisticsViewModel> Handle(GetStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        var expenses = await _db.Expenses
            .Where(e => e.UserId == query.UserId)
            .OrderBy(e => e.OccurredAt)
            .ToListAsync(cancellationToken);

        var culture = new CultureInfo("ru-RU");

        var fuelExpenses = expenses.Where(e => e.Type == ExpenseType.Fuel).ToList();
        var serviceExpenses = expenses.Where(e => e.Type == ExpenseType.Service).ToList();
        var otherExpenses = expenses.Where(e => e.Type == ExpenseType.Other).ToList();

        var fuelStats = BuildFuelStatistics(fuelExpenses, culture);
        var consumptionStats = BuildConsumptionStatistics(fuelExpenses);
        var otherStats = BuildOtherStatistics(serviceExpenses, otherExpenses);

        return new StatisticsViewModel
        {
            Fuel = fuelStats,
            Consumption = consumptionStats,
            Other = otherStats
        };
    }

    private static FuelStatistics BuildFuelStatistics(List<Expense> fuelExpenses, CultureInfo culture)
    {
        if (fuelExpenses.Count == 0)
        {
            return new FuelStatistics();
        }

        var fillUps = fuelExpenses.Count;
        var totalLiters = fuelExpenses.Sum(e => e.Liters ?? 0m);
        var totalAmount = fuelExpenses.Sum(e => e.Amount);
        var avgPrice = totalLiters > 0 ? totalAmount / totalLiters : 0m;

        var monthlyGroups = fuelExpenses
            .GroupBy(e => new { e.OccurredAt.Year, e.OccurredAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .ToList();

        var lastSix = monthlyGroups.TakeLast(6).ToList();
        var maxLiters = lastSix.Max(g => g.Sum(e => e.Liters ?? 0m));
        if (maxLiters <= 0)
        {
            maxLiters = 1;
        }

        var monthly = lastSix
            .Select(g =>
            {
                var liters = g.Sum(e => e.Liters ?? 0m);
                var amount = g.Sum(e => e.Amount);
                var height = (int)Math.Round((double)(liters / maxLiters * 100m));
                var monthName = culture.DateTimeFormat.AbbreviatedMonthNames[g.Key.Month - 1];
                return new MonthlyFuelStat
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = monthName,
                    TotalLiters = liters,
                    TotalAmount = amount,
                    HeightPercent = height
                };
            })
            .ToList();

        return new FuelStatistics
        {
            FillUps = fillUps,
            TotalLiters = totalLiters,
            TotalAmount = totalAmount,
            AveragePricePerLiter = avgPrice,
            Monthly = monthly
        };
    }

    private static ConsumptionStatistics BuildConsumptionStatistics(List<Expense> fuelExpenses)
    {
        var withMileage = fuelExpenses
            .Where(e => e.Mileage.HasValue && e.Liters.HasValue)
            .OrderBy(e => e.Mileage!.Value)
            .ToList();

        if (withMileage.Count < 2)
        {
            return new ConsumptionStatistics();
        }

        var minMileage = withMileage.First().Mileage!.Value;
        var maxMileage = withMileage.Last().Mileage!.Value;
        var totalDistance = maxMileage - minMileage;
        if (totalDistance <= 0)
        {
            return new ConsumptionStatistics();
        }

        var totalLiters = withMileage.Sum(e => e.Liters ?? 0m);
        var totalAmount = withMileage.Sum(e => e.Amount);

        decimal? avgConsumption = totalDistance > 0 ? totalLiters / totalDistance * 100m : null;
        var avgPrice = totalLiters > 0 ? totalAmount / totalLiters : (decimal?)null;
        decimal? costPer100 = null;
        if (avgConsumption.HasValue && avgPrice.HasValue)
        {
            costPer100 = avgConsumption.Value * avgPrice.Value;
        }

        var monthlyGroups = withMileage
            .GroupBy(e => new { e.OccurredAt.Year, e.OccurredAt.Month })
            .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
            .Take(3)
            .ToList();

        var monthly = new List<MonthlyConsumptionStat>();

        foreach (var g in monthlyGroups)
        {
            var minM = g.Min(e => e.Mileage!.Value);
            var maxM = g.Max(e => e.Mileage!.Value);
            var dist = maxM - minM;
            var liters = g.Sum(e => e.Liters ?? 0m);
            var cons = dist > 0 ? liters / dist * 100m : 0m;
            monthly.Add(new MonthlyConsumptionStat
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                ConsumptionLPer100Km = cons
            });
        }

        var maxCons = monthly.Any() ? monthly.Max(m => m.ConsumptionLPer100Km) : 0m;
        if (maxCons <= 0)
        {
            maxCons = 1;
        }

        foreach (var m in monthly)
        {
            m.WidthPercent = (int)Math.Round((double)(m.ConsumptionLPer100Km / maxCons * 100m));
        }

        return new ConsumptionStatistics
        {
            AverageConsumptionLPer100Km = avgConsumption,
            TotalDistanceKm = totalDistance,
            AverageFuelPrice = avgPrice,
            CostPer100Km = costPer100,
            Monthly = monthly
        };
    }

    private static OtherExpensesStatistics BuildOtherStatistics(List<Expense> serviceExpenses, List<Expense> otherExpenses)
    {
        var serviceTotal = serviceExpenses.Sum(e => e.Amount);
        var otherTotal = otherExpenses.Sum(e => e.Amount);

        var serviceByKind = serviceExpenses
            .Where(e => !string.IsNullOrWhiteSpace(e.ServiceKind))
            .GroupBy(e => e.ServiceKind!)
            .Select(g => new CategoryStat
            {
                Name = g.Key,
                Amount = g.Sum(e => e.Amount)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        var otherByTitle = otherExpenses
            .Where(e => !string.IsNullOrWhiteSpace(e.Title))
            .GroupBy(e => e.Title!)
            .Select(g => new CategoryStat
            {
                Name = g.Key,
                Amount = g.Sum(e => e.Amount)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new OtherExpensesStatistics
        {
            ServiceTotal = serviceTotal,
            OtherTotal = otherTotal,
            ServiceByKind = serviceByKind,
            OtherByTitle = otherByTitle
        };
    }
}
