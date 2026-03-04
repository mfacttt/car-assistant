using CarAssistant.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dashboard.Commands;

public record DeleteNotificationCommand(int Id, int UserId);

public class DeleteNotificationCommandHandler
{
    private readonly ApplicationDbContext _db;

    public DeleteNotificationCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == command.Id && n.UserId == command.UserId, cancellationToken);

        if (notification is null)
        {
            return;
        }

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
