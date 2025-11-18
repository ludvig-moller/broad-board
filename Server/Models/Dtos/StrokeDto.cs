using backend.Models.Entities;
using System.Diagnostics.CodeAnalysis;

namespace backend.Models.Dtos;

public class StrokeDto
{
    public required Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public required string Color { get; set; }
    public required int LineWidth { get; set; }
    public List<PointDto> Points { get; set; } = [];

    public StrokeDto() { }

    [SetsRequiredMembers]
    public StrokeDto(Stroke s) 
    {
        Id = s.Id;
        UserId = s.UserId;
        Color = s.Color;
        LineWidth = s.LineWidth;
        Points = s.Points.Select(p => new PointDto(p)).ToList();
    }

    public Stroke ToEntity(Guid boardId)
        => new Stroke
        {
            Id = Id,
            UserId = UserId,
            Color = Color,
            LineWidth = LineWidth,
            Points = Points.Select(p => p.ToEntity(Id)).ToList(),
            BoardId = boardId,
        };
}
