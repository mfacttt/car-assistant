using System.Security.Claims;
using CarAssistant.Features.Expenses.Commands;
using CarAssistant.Features.Expenses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Controllers;

[Authorize]
public class ConsumationController : Controller
{
    private readonly SaveExpenseCommandHandler _saveExpense;

    public ConsumationController(SaveExpenseCommandHandler saveExpense)
    {
        _saveExpense = saveExpense;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(ExpenseInputModel model)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idStr, out var userId))
        {
            return RedirectToAction("Index", "Auth");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        if (!Enum.TryParse<ExpenseType>(model.Type, out var type))
        {
            type = ExpenseType.Other;
        }

        var occurredAt = model.Date.Date + model.Time;

        var command = new SaveExpenseCommand(
            userId,
            type,
            model.ServiceKind,
            model.Title,
            model.Amount,
            model.Liters,
            model.PricePerLiter,
            model.Mileage,
            model.FuelType,
            model.Comment,
            occurredAt);

        await _saveExpense.Handle(command);

        return RedirectToAction("Index", "Home");
    }
}