using CarAssistant.Data;
using CarAssistant.Features.Auth.Commands;
using CarAssistant.Features.Dairy.Commands;
using CarAssistant.Features.Dairy.Queries;
using CarAssistant.Features.CarProfile.Commands;
using CarAssistant.Features.CarProfile.Queries;
using CarAssistant.Features.Expenses.Commands;
using CarAssistant.Features.Expenses.Queries;
using CarAssistant.Features.Dashboard.Commands;
using CarAssistant.Features.Dashboard.Queries;
using CarAssistant.Features.Statistics.Queries;
using CarAssistant.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
// builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
// Используем одну и ту же persistent БД (SQLite) и в dev, и в prod,
// чтобы поведение было идентичным и данные не "терялись".
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=carassistant.db";

Console.WriteLine($"ASPNETCORE_ENVIRONMENT = {builder.Environment.EnvironmentName}");
Console.WriteLine("Using SQLite connection string: " + connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<LoginUserCommandHandler>();
builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<GetDairyEntriesQueryHandler>();
builder.Services.AddScoped<SaveDairyEntryCommandHandler>();
builder.Services.AddScoped<DeleteDairyEntryCommandHandler>();
builder.Services.AddScoped<SaveCarCommandHandler>();
builder.Services.AddScoped<GetUserCarQueryHandler>();
builder.Services.AddScoped<SaveExpenseCommandHandler>();
builder.Services.AddScoped<DeleteExpenseCommandHandler>();
builder.Services.AddScoped<GetLatestExpensesQueryHandler>();
builder.Services.AddScoped<GetNotificationsQueryHandler>();
builder.Services.AddScoped<CreateNotificationCommandHandler>();
builder.Services.AddScoped<DeleteNotificationCommandHandler>();
builder.Services.AddScoped<GetStatisticsQueryHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var wellKnownPath = Path.Combine(builder.Environment.WebRootPath, ".well-known");
if (Directory.Exists(wellKnownPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(wellKnownPath),
        RequestPath = "/.well-known",
        ServeUnknownFileTypes = true,
        DefaultContentType = "application/json"
    });
}

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

// Создаём БД и таблицы при запуске (для SQLite)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine("EF Core provider: " + db.Database.ProviderName);
    db.Database.EnsureCreated();
}

app.UseMiddleware<EnsureCarProfileMiddleware>();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Index}/{id?}");


app.Run();