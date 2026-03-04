using System.Security.Claims;
using CarAssistant.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Infrastructure;

public class EnsureCarProfileMiddleware
{
    private readonly RequestDelegate _next;

    public EnsureCarProfileMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Если пользователь не аутентифицирован, просто пропускаем запрос дальше.
        // Редирект на логин/авторизацию обрабатывается стандартной аутентификацией/атрибутами [Authorize].
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // Разрешаем доступ к аутентификации и статикам без проверки машины
        if (path.StartsWith("/Auth", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/img", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var idStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idStr, out var userId))
        {
            // если пользователя с таким Id нет в БД (InMemory очищена, а кука осталась) — выходим из сессии и отправляем на логин
            var userExists = await db.Users.AnyAsync(u => u.Id == userId, context.RequestAborted);
            if (!userExists)
            {
                // сбросить куку аутентификации и вернуть на страницу входа
                context.Response.Redirect("/Auth/Index");
                return;
            }

            var car = await db.Cars.FirstOrDefaultAsync(c => c.UserId == userId, context.RequestAborted);

            var hasCar = car is not null &&
                         !string.IsNullOrWhiteSpace(car.Brand) &&
                         !string.IsNullOrWhiteSpace(car.Model) &&
                         !string.IsNullOrWhiteSpace(car.FuelType) &&
                         !string.IsNullOrWhiteSpace(car.Vin) &&
                         car.Mileage > 0 &&
                         car.Year > 0 &&
                         car.EngineVolume > 0;

            if (!hasCar)
            {
                context.Response.Redirect("/Auth/Welcome");
                return;
            }
        }

        await _next(context);
        
    }
}
