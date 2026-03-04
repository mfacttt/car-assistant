using System.Collections.Generic;

namespace CarAssistant.Features.Dairy.Models;

public class DairyEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public ICollection<DairyPhoto> Photos { get; set; } = new List<DairyPhoto>();
}
