using backend.Models.Dtos;

namespace backend.Messages;

public class BoardMessage
{
    public required string Type { get; set; }
    public string? ErrorMessage { get; set; }
    public List<StrokeDto>? Strokes { get; set; }
    public StrokeDto? Stroke { get; set; }
    public string? StrokeId { get; set; }
    public PointDto? Point { get; set; }
    public string? UserId { get; set; }
}
