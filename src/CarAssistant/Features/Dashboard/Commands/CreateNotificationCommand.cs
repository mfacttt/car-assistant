using CarAssistant.Data;
using CarAssistant.Features.Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dashboard.Commands;

public record CreateNotificationCommand(int? Id, string Type, DateTime Date, int UserId);

public class CreateNotificationCommandHandler
{
    private readonly ApplicationDbContext _db;

    public CreateNotificationCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Type))
        {
            return;
        }

        Notification? notification = null;

        if (command.Id.HasValue && command.Id.Value > 0)
        {
            notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == command.Id.Value && n.UserId == command.UserId, cancellationToken);
        }

        if (notification is null)
        {
            notification = new Notification
            {
                UserId = command.UserId
            };

            _db.Notifications.Add(notification);
        }

        notification.Type = command.Type.Trim();
        notification.Date = command.Date;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
