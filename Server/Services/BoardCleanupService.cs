using backend.Managers;
using backend.Models.Entities;

namespace backend.Services;

public class BoardCleanupService(IServiceProvider serviceProvider, BoardManager boardManager) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly BoardManager _boardManager = boardManager;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            CleanupBoards();
            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }

    private void CleanupBoards()
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<BoardService>();

        var expiredBoards = service.GetBoards()
            .Where(b => b.ExpirationDate < DateTime.UtcNow);

        foreach (var board in expiredBoards)
        {
            var boardSession = _boardManager.GetOrCreateSession(board.Id);

            if (boardSession.Clients.Count > 0)
            {
                var boardFound = service.ExtendBoardExpiration(board.Id, TimeSpan.FromMinutes(15));

                if (!boardFound)
                    RemoveBoard(service, board);
            }
            else
            {
                RemoveBoard(service, board);
            }
        }
    }

    private void RemoveBoard(BoardService service, Board board)
    {
        _boardManager.RemoveSession(board.Id);
        service.RemoveBoard(board.Id);
    }
}
