using System.Net.WebSockets;

namespace backend.Models;

public class Board
{
    public required string Id { get; set; }
    public List<Stroke> Strokes { get; set; } = [];
    public List<WebSocket> Clients { get; set; } = [];
}
