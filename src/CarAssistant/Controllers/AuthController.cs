using System.Security.Claims;
using CarAssistant.Features.Auth.Commands;
using CarAssistant.Features.Auth.Models;
using CarAssistant.Features.CarProfile.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Controllers;

public class AuthController : Controller
{
    private readonly LoginUserCommandHandler _loginHandler;
    private readonly RegisterUserCommandHandler _registerHandler;
    private readonly SaveCarCommandHandler _saveCarCommandHandler;

    public AuthController(
        LoginUserCommandHandler loginHandler,
        RegisterUserCommandHandler registerHandler,
        SaveCarCommandHandler saveCarCommandHandler)
    {
        _loginHandler = loginHandler;
        _registerHandler = registerHandler;
        _saveCarCommandHandler = saveCarCommandHandler;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Index");
        }

        var user = await _loginHandler.Handle(new LoginUserCommand(model.Email, model.Password));

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Неверная почта или пароль.");
            ViewData["ReturnUrl"] = returnUrl;
            return View("Index");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        var (success, error, user) = await _registerHandler.Handle(new RegisterUserCommand(model.Email, model.Password));

        if (!success || user is null)
        {
            ModelState.AddModelError(string.Empty, error ?? "Не удалось создать пользователя.");
            return View("Index");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Welcome");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Welcome()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SaveCar(string brand, string model, string fuelType, string vin, int mileage, int year, decimal engineVolume)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Index");
        }

        await _saveCarCommandHandler.Handle(new SaveCarCommand(userId, brand, model, fuelType, vin, mileage, year, engineVolume));
        return RedirectToAction("Index", "Home");
    }
}