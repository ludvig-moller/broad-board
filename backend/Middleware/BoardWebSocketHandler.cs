using System.Net.WebSockets;
using System.Text;
using backend.Services;
using backend.Models;

namespace backend.Middleware;

public class BoardWebSocketHandler(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next(context);
            return;
        }

        var boardId = context.Request.Query["boardId"].ToString();
        if (string.IsNullOrEmpty(boardId)) 
        {
            context.Response.StatusCode = 400;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        var board = BoardStore.Boards.GetValueOrDefault(boardId);

        if (board == null)
        {
            board = new Board { Id = boardId };
            BoardStore.Boards[boardId] = board;
        }

        board.Clients.Add(webSocket);

        var initPayload = System.Text.Json.JsonSerializer.Serialize(new { type = "init", strokes = board.Strokes });
        await webSocket.SendAsync(Encoding.UTF8.GetBytes(initPayload), WebSocketMessageType.Text, true, CancellationToken.None);

        await ReciveLoop(board, webSocket);

        board.Clients.Remove(webSocket);
    }

    private async Task ReciveLoop(Board board, WebSocket webSocket)
    {
        var buffer = new byte[4096];
        while (webSocket.State == WebSocketState.Open) 
        {
            // Add reciving message logic
        }
    }
}
