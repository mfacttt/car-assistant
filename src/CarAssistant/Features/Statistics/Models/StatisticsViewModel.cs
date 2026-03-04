using System.Collections.Generic;

namespace CarAssistant.Features.Statistics.Models;

public class StatisticsViewModel
{
    public FuelStatistics Fuel { get; set; } = new();
    public ConsumptionStatistics Consumption { get; set; } = new();
    public OtherExpensesStatistics Other { get; set; } = new();
}

public class FuelStatistics
{
    public int FillUps { get; set; }
    public decimal TotalLiters { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AveragePricePerLiter { get; set; }
    public IReadOnlyList<MonthlyFuelStat> Monthly { get; set; } = new List<MonthlyFuelStat>();
}

public class MonthlyFuelStat
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalLiters { get; set; }
    public decimal TotalAmount { get; set; }
    public int HeightPercent { get; set; }
}

public class ConsumptionStatistics
{
    public decimal? AverageConsumptionLPer100Km { get; set; }
    public int? TotalDistanceKm { get; set; }
    public decimal? AverageFuelPrice { get; set; }
    public decimal? CostPer100Km { get; set; }
    public IReadOnlyList<MonthlyConsumptionStat> Monthly { get; set; } = new List<MonthlyConsumptionStat>();
}

public class MonthlyConsumptionStat
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal ConsumptionLPer100Km { get; set; }
    public int WidthPercent { get; set; }
}

public class OtherExpensesStatistics
{
    public decimal ServiceTotal { get; set; }
    public decimal OtherTotal { get; set; }
    public IReadOnlyList<CategoryStat> ServiceByKind { get; set; } = new List<CategoryStat>();
    public IReadOnlyList<CategoryStat> OtherByTitle { get; set; } = new List<CategoryStat>();
}

public class CategoryStat
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
