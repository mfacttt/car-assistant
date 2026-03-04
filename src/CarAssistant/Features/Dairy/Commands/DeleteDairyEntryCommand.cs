using CarAssistant.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dairy.Commands;

public record DeleteDairyEntryCommand(int Id, int UserId);

public class DeleteDairyEntryCommandHandler
{
    private readonly ApplicationDbContext _db;

    public DeleteDairyEntryCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteDairyEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await _db.DairyEntries
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == command.Id && e.UserId == command.UserId, cancellationToken);

        if (entry is null)
        {
            return;
        }

        if (entry.Photos is { Count: > 0 })
        {
            _db.DairyPhotos.RemoveRange(entry.Photos);
        }

        _db.DairyEntries.Remove(entry);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
