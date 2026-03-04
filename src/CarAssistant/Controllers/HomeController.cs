using System.Security.Claims;
using CarAssistant.Features.CarProfile.Commands;
using CarAssistant.Features.CarProfile.Queries;
using CarAssistant.Features.Expenses.Commands;
using CarAssistant.Features.Expenses.Queries;
using CarAssistant.Features.Dashboard.Commands;
using CarAssistant.Features.Dashboard.Models;
using CarAssistant.Features.Dashboard.Queries;
using CarAssistant.Features.Statistics.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GetUserCarQueryHandler _getUserCar;
    private readonly SaveCarCommandHandler _saveCar;
    private readonly GetLatestExpensesQueryHandler _getLatestExpenses;
    private readonly DeleteExpenseCommandHandler _deleteExpense;
    private readonly GetNotificationsQueryHandler _getNotifications;
    private readonly CreateNotificationCommandHandler _createNotification;
    private readonly DeleteNotificationCommandHandler _deleteNotification;
    private readonly GetStatisticsQueryHandler _getStatistics;

    public HomeController(
        ILogger<HomeController> logger,
        GetUserCarQueryHandler getUserCar,
        SaveCarCommandHandler saveCar,
        GetLatestExpensesQueryHandler getLatestExpenses,
        GetNotificationsQueryHandler getNotifications,
        CreateNotificationCommandHandler createNotification,
        DeleteNotificationCommandHandler deleteNotification,
        DeleteExpenseCommandHandler deleteExpense,
        GetStatisticsQueryHandler getStatistics)
    {
        _logger = logger;
        _getUserCar = getUserCar;
        _saveCar = saveCar;
        _getLatestExpenses = getLatestExpenses;
        _getNotifications = getNotifications;
        _createNotification = createNotification;
        _deleteNotification = deleteNotification;
        _deleteExpense = deleteExpense;
        _getStatistics = getStatistics;
    }

    [HttpPost]
    public async Task<IActionResult> SaveCarFromHome(string brand, string model, string fuelType, int year, string vin, int mileage, decimal engineVolume)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Index", "Auth");
        }

        await _saveCar.Handle(new SaveCarCommand(userId.Value, brand, model, fuelType, vin, mileage, year, engineVolume));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Index", "Auth");
        }

        var model = new HomeDashboardViewModel
        {
            Car = await _getUserCar.Handle(new GetUserCarQuery(userId.Value)),
            Notifications = await _getNotifications.Handle(new GetNotificationsQuery(userId.Value)),
            LatestExpenses = await _getLatestExpenses.Handle(new GetLatestExpensesQuery(userId.Value, 4))
        };

        var stats = await _getStatistics.Handle(new GetStatisticsQuery(userId.Value));
        model.AverageConsumptionLPer100Km = stats.Consumption.AverageConsumptionLPer100Km;
        model.TotalDistanceKm = stats.Consumption.TotalDistanceKm;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddNotification(int? id, string type, DateTime date)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Index", "Auth");
        }

        await _createNotification.Handle(new CreateNotificationCommand(id, type, date, userId.Value));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Index", "Auth");
        }

        await _deleteNotification.Handle(new DeleteNotificationCommand(id, userId.Value));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Index", "Auth");
        }

        await _deleteExpense.Handle(new DeleteExpenseCommand(id, userId.Value));
        return RedirectToAction(nameof(Index));
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idClaim, out var id))
        {
            return id;
        }

        return null;
    }
}