using System.Net.WebSockets;

namespace backend.Models;

public class Board
{
    public required string Id { get; set; }
    public List<Stroke> Strokes { get; set; } = [];
    public List<WebSocket> Clients { get; set; } = [];

    public void AddStroke(Stroke stroke)
    {
        Strokes.Add(stroke);
    }

    public bool AddPointToStroke(string strokeId, Point point)
    {
        var stroke = Strokes.FirstOrDefault(s => s.Id == strokeId);
        if (stroke == null) return false;

        stroke.Points.Add(point);
        return true;
    }

    public void ClearBoard()
    {
        Strokes.Clear();
    }
}
