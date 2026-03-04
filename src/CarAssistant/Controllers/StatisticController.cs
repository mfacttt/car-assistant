using System.Security.Claims;
using CarAssistant.Features.Statistics.Models;
using CarAssistant.Features.Statistics.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Controllers;

[Authorize]
public class StatisticController : Controller
{
    private readonly GetStatisticsQueryHandler _getStatistics;

    public StatisticController(GetStatisticsQueryHandler getStatistics)
    {
        _getStatistics = getStatistics;
    }

    public async Task<IActionResult> Index()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idStr, out var userId))
        {
            return RedirectToAction("Index", "Auth");
        }

        StatisticsViewModel model = await _getStatistics.Handle(new GetStatisticsQuery(userId));
        return View(model);
    }
}