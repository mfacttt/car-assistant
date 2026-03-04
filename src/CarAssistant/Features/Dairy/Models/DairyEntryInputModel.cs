using System.ComponentModel.DataAnnotations;

namespace CarAssistant.Features.Dairy.Models;

public class DairyEntryInputModel
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;
}
