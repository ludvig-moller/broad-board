namespace backend.Models;

public class Point { 
    public float X { get; set; }
    public float Y { get; set; }
}
public class Stroke
{
    public required string UserId { get; set; }
    public required string Id { get; set; }
    public required string Color { get; set; }
    public required int LineWidth { get; set; }
    public List<Point> Points { get; set; } = [];
}
