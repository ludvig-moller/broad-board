using backend.Models;

namespace backend.Messages;

public class BoardMessage
{
    public required string Type { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Stroke>? Strokes { get; set; }
    public Stroke? Stroke { get; set; }
    public string? StrokeId { get; set; }
    public Point? Point { get; set; }
}
