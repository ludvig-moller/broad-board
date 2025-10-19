using backend.Models;

namespace backend.Services;

public static class BoardStore
{
    public static Dictionary<string, Board> Boards { get; } = [];
    public static void ExpiredBoard(string boardId)
    {
        if (!Boards.TryGetValue(boardId, out Board? board)) return;

        if (board.Clients.Count != 0)
        {
            board.ResetExpiration();
        }
        else
        {
            Boards.Remove(boardId);
        }
    }
}
