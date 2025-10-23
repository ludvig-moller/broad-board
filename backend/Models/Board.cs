using System.Net.WebSockets;

namespace backend.Models;

public class Board
{
    public string Id { get; }
    public List<Stroke> Strokes { get; }
    public List<WebSocket> Clients { get; }
    private Timer? _expirationTimer;
    private readonly Action<string> _onExpired;

    public Board(string id, Action<string> onExpired)
    {
        Id = id;

        Strokes = [];
        Clients = [];

        _onExpired = onExpired;

        StartExpirationTimer();
    }
    
    public void StartExpirationTimer()
    {
        Timer? timer = null;
        timer = new Timer(_ =>
        { 
            _onExpired?.Invoke(Id);
            timer?.Dispose();
        }, null, TimeSpan.FromHours(1), Timeout.InfiniteTimeSpan);

        _expirationTimer = timer;
    }

    public void ResetExpiration()
    {
        _expirationTimer?.Dispose();
        StartExpirationTimer();
    }

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

    public bool RemoveUsersLastStroke(string userId)
    {
        var stroke = Strokes.LastOrDefault(s => s.UserId == userId);
        if (stroke == null) return false;

        Strokes.Remove(stroke);
        return true;
    }

    public void ClearBoard()
    {
        Strokes.Clear();
    }
}
