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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CarAssistantDb"));

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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<EnsureCarProfileMiddleware>();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Index}/{id?}");


app.Run();