namespace backend.Models.Entities;

public class Stroke
{
    public required string Id { get; set; }
    public string? UserId { get; set; }
    public required string Color { get; set; }
    public required int LineWidth { get; set; }
    public ICollection<Point> Points { get; set; } = [];

    public required string BoardId { get; set; }
}
