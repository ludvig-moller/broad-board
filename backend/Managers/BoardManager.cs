using System.Collections.Concurrent;

namespace backend.Managers;

public class BoardManager
{
    private readonly ConcurrentDictionary<string, BoardSession> _sessions = [];

    public BoardSession GetOrCreateSession(string boardId)
        => _sessions.GetOrAdd(boardId, id => new BoardSession(id));

    public void RemoveSession(string boardId)
        => _sessions.TryRemove(boardId, out _);

}
