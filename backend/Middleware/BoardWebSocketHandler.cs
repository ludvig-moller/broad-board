using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using backend.Services;
using backend.Models;
using backend.Messages;

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

        using var clientSocket = await context.WebSockets.AcceptWebSocketAsync();

        var board = BoardStore.Boards.GetValueOrDefault(boardId);

        if (board == null)
        {
            board = new Board { Id = boardId };
            BoardStore.Boards[boardId] = board;
        }

        board.Clients.Add(clientSocket);

        var initPayload = JsonSerializer.Serialize(new { Type = "init", board.Strokes }, Config.Json.Options);
        await clientSocket.SendAsync(Encoding.UTF8.GetBytes(initPayload), WebSocketMessageType.Text, true, CancellationToken.None);

        await ReciveLoop(clientSocket, board);

        board.Clients.Remove(clientSocket);
    }

    private async Task ReciveLoop(WebSocket clientSocket, Board board)
    {
        var buffer = new byte[4096];
        while (clientSocket.State == WebSocketState.Open) 
        {
            var result = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientSocket.CloseAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription, CancellationToken.None);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await HandleMessage(clientSocket, message, board);
            
        }
    }

    private async Task HandleMessage(WebSocket clientSocket, string messageString, Board board)
    {
        BoardMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<BoardMessage>(messageString, Config.Json.Options);
        }
        catch
        {
            // Handle invalid JSON
            return;
        }

        if (message == null) return;

        switch (message.Type) 
        {
            case "addStroke":
                await HandleAddStroke(clientSocket, message, board);
                break;

            case "addPointToStroke":
                await HandleAddPointToStroke(clientSocket, message, board);
                break;

            default:

                break;
        }
    }

    private Task HandleAddStroke(WebSocket clientSocket, BoardMessage message, Board board)
    { 
        // Handle errors

        board.AddStroke(message.Stroke);

        return BroadCastToOthers(board.Clients, clientSocket, message);
    }

    private Task HandleAddPointToStroke(WebSocket clientSocket, BoardMessage message, Board board)
    {
        // Handle errors

        board.AddPointToStroke(message.StrokeId, message.Point);

        return BroadCastToOthers(board.Clients, clientSocket, message);
    }

    private static async Task BroadCastToOthers(List<WebSocket> clients, WebSocket sender, BoardMessage message)
    {
        var payload = JsonSerializer.Serialize(message, Config.Json.Options);
        var bytes = Encoding.UTF8.GetBytes(payload);

        foreach (var client in clients.Where(c => c != sender && c.State == WebSocketState.Open)) 
        { 
            await client.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
