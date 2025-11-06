using System.Collections.Concurrent;

namespace backend.Managers;

public class BoardManager
{
    private readonly ConcurrentDictionary<Guid, BoardSession> _sessions = [];

    public BoardSession GetOrCreateSession(Guid boardId)
        => _sessions.GetOrAdd(boardId, id => new BoardSession(id));

    public void RemoveSession(Guid boardId)
        => _sessions.TryRemove(boardId, out _);

}
