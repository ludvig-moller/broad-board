namespace backend.Models.Entities;

public class Stroke
{
    public required Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public required string Color { get; set; }
    public required int LineWidth { get; set; }
    public ICollection<Point> Points { get; set; } = [];

    public required Guid BoardId { get; set; }
}
