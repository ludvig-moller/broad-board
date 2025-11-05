using backend.Models.Entities;

namespace backend.Models.Dtos;

public class PointDto
{
    public float X { get; set; }
    public float Y { get; set; }

    public PointDto() { }
    public PointDto(Point p)
    {
        X = p.X;
        Y = p.Y;
    }

    public Point ToEntity(string strokeId)
        => new Point 
        { 
            StrokeId = strokeId,
            X = X,
            Y = Y,
        };
}
