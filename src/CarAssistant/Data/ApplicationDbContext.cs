using CarAssistant.Features.Auth.Models;
using CarAssistant.Features.CarProfile.Models;
using CarAssistant.Features.Dairy.Models;
using CarAssistant.Features.Dashboard.Models;
using CarAssistant.Features.Expenses.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<DairyEntry> DairyEntries => Set<DairyEntry>();
    public DbSet<DairyPhoto> DairyPhotos => Set<DairyPhoto>();
    public DbSet<Notification> Notifications => Set<Notification>();
}
