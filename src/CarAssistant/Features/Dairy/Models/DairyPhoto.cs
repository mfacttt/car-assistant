namespace CarAssistant.Features.Dairy.Models;

public class DairyPhoto
{
    public int Id { get; set; }
    public int DairyEntryId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
}
