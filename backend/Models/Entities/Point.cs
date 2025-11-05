using Microsoft.EntityFrameworkCore;

namespace backend.Models.Entities;

[Owned]
public class Point
{
    public float X { get; set; } 
    public float Y { get; set; }

    public required string StrokeId { get; set; }
}
