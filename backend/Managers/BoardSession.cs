using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace backend.Managers;

public class BoardSession(string boardId)
{
    public string BoardId { get; set; } = boardId;
    private readonly ConcurrentDictionary<WebSocket, bool> _clients = [];

    public IList<WebSocket> Clients => _clients.Keys.ToList();

    public void AddClient(WebSocket client) =>
        _clients.TryAdd(client, true);

    public void RemoveClient(WebSocket client) =>
        _clients.TryRemove(client, out _);
}
