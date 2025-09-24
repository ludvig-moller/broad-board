using backend.Models;

namespace backend.Messages;

public class BoardMessage
{
    public string Type { get; set; }
    public Stroke Stroke { get; set; }
    public string StrokeId { get; set; }
    public Point Point { get; set; }
}
