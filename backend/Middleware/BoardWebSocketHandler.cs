using backend.Messages;
using backend.Models;
using backend.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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

        var board = BoardStore.Boards.GetValueOrDefault(boardId)
            ?? new Board(boardId, BoardStore.ExpiredBoard);

        BoardStore.Boards[boardId] = board;
        board.Clients.Add(clientSocket);

        try
        {
            BoardMessage initMessage = new() { Type = "init", Strokes = board.Strokes };
            var initPayload = JsonSerializer.Serialize(initMessage, Config.Json.Options);
            await clientSocket.SendAsync(Encoding.UTF8.GetBytes(initPayload), WebSocketMessageType.Text, true, CancellationToken.None);

            await ReciveLoop(clientSocket, board);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Got Exeption in WebSocket: {ex}");
        }
        finally
        {
            board.Clients.Remove(clientSocket);

            if (clientSocket.State is WebSocketState.Open 
                or WebSocketState.CloseReceived 
                or WebSocketState.CloseSent)
            {
                try 
                { 
                    await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None); 
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"Got Exeption when closing socket: {ex}"); 
                }
            }

            clientSocket.Dispose();
        }
    }

    private static async Task ReciveLoop(WebSocket clientSocket, Board board)
    {
        var buffer = new byte[4096];
        while (clientSocket.State == WebSocketState.Open) 
        {
            WebSocketReceiveResult result;
            try
            {
                result = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (WebSocketException)
            {
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientSocket.CloseAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription, 
                    CancellationToken.None);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await HandleMessage(clientSocket, message, board);
        }
    }

    private static async Task HandleMessage(WebSocket clientSocket, string messageString, Board board)
    {
        BoardMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<BoardMessage>(messageString, Config.Json.Options);
        }
        catch
        {
            await SendErrorMessageToClient(clientSocket, "Got invalid JSON.");
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

            case "undo":
                await HandleUndo(clientSocket, message, board);
                break;

            case "clearBoard":
                await HandleClearBoard(clientSocket, message, board);
                break;

            default:
                BoardMessage errorMessage = new() { Type = "error", ErrorMessage = "Got a unknown type needs to be addStroke or addPointToStroke" };
                var errorPayload = JsonSerializer.Serialize(errorMessage, Config.Json.Options);
                await clientSocket.SendAsync(Encoding.UTF8.GetBytes(errorPayload), WebSocketMessageType.Text, true, CancellationToken.None);
                break;
        }
    }

    private static Task HandleAddStroke(WebSocket clientSocket, BoardMessage message, Board board)
    { 
        if (message.Stroke == null) 
        {
            return SendErrorMessageToClient(
                clientSocket,
                "Type addStroke needs a stroke value but didn't get one.");
        }

        board.AddStroke(message.Stroke);

        return BroadCastToOthers(board.Clients, clientSocket, message);
    }

    private static Task HandleAddPointToStroke(WebSocket clientSocket, BoardMessage message, Board board)
    {
        if (message.StrokeId == null || message.Point == null)
        {
            return SendErrorMessageToClient(
                clientSocket,
                "Type addPointToStroke needs a strokeId and point value but didn't get them.");
        }

        board.AddPointToStroke(message.StrokeId, message.Point);

        return BroadCastToOthers(board.Clients, clientSocket, message);
    }

    private static Task HandleUndo(WebSocket clientSocket, BoardMessage message, Board board)
    {
        if (message.UserId == null)
        {
            return SendErrorMessageToClient(
                clientSocket, 
                "Type undo needs a userId but didn't get one.");
        }

        board.RemoveUsersLastStroke(message.UserId);

        return BroadCastToOthers(board.Clients, clientSocket, message);
    }

    private static Task HandleClearBoard(WebSocket clientSocket, BoardMessage message, Board board)
    {
        board.ClearBoard();

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

    private static async Task SendErrorMessageToClient(WebSocket clientSocket, string errorMessage)
    {
        BoardMessage message = new() { Type = "error", ErrorMessage = errorMessage };
        var payload = JsonSerializer.Serialize(message, Config.Json.Options);
        var bytes = Encoding.UTF8.GetBytes(payload);

        await clientSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
