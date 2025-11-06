using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class BoardService(BoardDbContext context)
{
    private readonly BoardDbContext _context = context;

    public Board? GetBoard(Guid boardId) 
        => _context.Boards.FirstOrDefault(b => b.Id == boardId);

    public List<Board> GetBoards()
        => _context.Boards.ToList();

    public void AddBoard(Board board)
    {
        _context.Boards.Add(board);
        _context.SaveChanges();
    }

    public void RemoveBoard(Guid boardId)
    {
        var board = _context.Boards
            .Include(b => b.Strokes)
            .FirstOrDefault(b => b.Id == boardId);

        if (board == null)
            return;

        _context.Boards.Remove(board);
        _context.SaveChanges();
    }

    public bool ExtendBoardExpiration(Guid boardId, TimeSpan duration)
    {
        var board = _context.Boards.FirstOrDefault(b => b.Id == boardId);

        if (board == null)
            return false;

        board.ExpirationDate = DateTime.UtcNow.Add(duration);
        _context.SaveChanges();
        return true;
    }

    public List<Stroke> GetStrokes(Guid boardId) 
        => _context.Strokes
            .Where(s => s.BoardId == boardId)
            .Include(s => s.Points)
            .ToList();

    public bool AddStroke(Guid boardId, Stroke stroke)
    {
        var board = _context.Boards.FirstOrDefault(b => b.Id == boardId);

        if (board == null) 
            return false;

        board.Strokes.Add(stroke);
        _context.SaveChanges();
        return true;
    }

    public bool AddPointToStroke(Guid strokeId, Point point)
    {
        var stroke = _context.Strokes.FirstOrDefault(s => s.Id == strokeId);

        if (stroke == null) 
            return false;

        stroke.Points.Add(point);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveUsersLastStroke(Guid userId)
    {
        var stroke = _context.Strokes.LastOrDefault(s => s.UserId == userId);

        if (stroke == null) 
            return false;

        _context.Strokes.Remove(stroke);
        _context.SaveChanges();
        return true;
    }

    public void ClearBoard(Guid boardId)
    {
        _context.Strokes.RemoveRange(_context.Strokes.Where(s => s.BoardId == boardId));
        _context.SaveChanges();
    }
}
