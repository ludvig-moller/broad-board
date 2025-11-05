using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class BoardService(BoardDbContext context)
{
    private readonly BoardDbContext _context = context;

    public Board? GetBoard(string boardId) =>
        _context.Boards.FirstOrDefault(b => b.Id == boardId);

    public void AddBoard(Board board)
    {
        _context.Boards.Add(board);
        _context.SaveChanges();
    }

    public List<Stroke> GetStrokes(string boardId) =>
        _context.Strokes
            .Where(s => s.BoardId == boardId)
            .Include(s => s.Points)
            .ToList();

    public bool AddStroke(string boardId, Stroke stroke)
    {
        var board = _context.Boards.FirstOrDefault(b => b.Id == boardId);

        if (board == null) 
            return false;

        board.Strokes.Add(stroke);
        _context.SaveChanges();
        return true;
    }

    public bool AddPointToStroke(string strokeId, Point point)
    {
        var stroke = _context.Strokes.FirstOrDefault(s => s.Id == strokeId);

        if (stroke == null) 
            return false;

        stroke.Points.Add(point);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveUsersLastStroke(string userId)
    {
        var stroke = _context.Strokes.LastOrDefault(s => s.UserId == userId);

        if (stroke == null) 
            return false;

        _context.Strokes.Remove(stroke);
        _context.SaveChanges();
        return true;
    }

    public void ClearBoard(string boardId)
    {
        _context.Strokes.RemoveRange(_context.Strokes.Where(s => s.BoardId == boardId));
        _context.SaveChanges();
    }
}
