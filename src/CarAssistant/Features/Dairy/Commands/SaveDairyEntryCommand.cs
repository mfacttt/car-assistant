using CarAssistant.Data;
using CarAssistant.Features.Dairy.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Dairy.Commands;

public record SaveDairyEntryCommand(int? Id, string Title, string Text, int UserId, IReadOnlyCollection<string> PhotoUrls);

public class SaveDairyEntryCommandHandler
{
    private readonly ApplicationDbContext _db;

    public SaveDairyEntryCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(SaveDairyEntryCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Title) || string.IsNullOrWhiteSpace(command.Text))
        {
            return;
        }

        DairyEntry? entry = null;

        if (command.Id is > 0)
        {
            entry = await _db.DairyEntries
                .Include(e => e.Photos)
                .FirstOrDefaultAsync(e => e.Id == command.Id && e.UserId == command.UserId, cancellationToken);
        }

        if (entry is null)
        {
            entry = new DairyEntry
            {
                UserId = command.UserId,
                Title = command.Title.Trim(),
                Text = command.Text.Trim(),
                Date = DateTime.UtcNow
            };

            _db.DairyEntries.Add(entry);
        }
        else
        {
            entry.Title = command.Title.Trim();
            entry.Text = command.Text.Trim();
            entry.Date = DateTime.UtcNow;
        }

        // Обновить фото: удалить старые и добавить до 10 новых URL
        var normalizedUrls = command.PhotoUrls
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct()
            .Take(10)
            .ToList();

        // Загрузить/очистить существующие фото
        if (entry.Id != 0)
        {
            var existingPhotos = await _db.DairyPhotos
                .Where(p => p.DairyEntryId == entry.Id)
                .ToListAsync(cancellationToken);

            if (existingPhotos.Count > 0)
            {
                _db.DairyPhotos.RemoveRange(existingPhotos);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (normalizedUrls.Count > 0)
        {
            var order = 0;
            foreach (var url in normalizedUrls)
            {
                _db.DairyPhotos.Add(new DairyPhoto
                {
                    DairyEntryId = entry.Id,
                    Url = url,
                    Order = order++
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
