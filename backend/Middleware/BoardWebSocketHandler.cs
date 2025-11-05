using backend.Managers;
using backend.Messages;
using backend.Models.Dtos;
using backend.Models.Entities;
using backend.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.Middleware;

public class BoardWebSocketHandler(RequestDelegate next, BoardManager manager)
{
    private readonly RequestDelegate _next = next;
    private readonly BoardManager _manager = manager;

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

        var service = context.RequestServices.GetRequiredService<BoardService>();

        var board = service.GetBoard(boardId);

        if (board == null)
        {
            board = new Board(boardId);
            service.AddBoard(board);
        }

        var session = _manager.GetOrCreateSession(boardId);
        session.AddClient(clientSocket);

        try
        {
            var strokes = service.GetStrokes(board.Id).Select(s => new StrokeDto(s)).ToList();

            BoardMessage initMessage = new() { Type = "init", Strokes = strokes };
            var initPayload = JsonSerializer.Serialize(initMessage, Config.Json.Options);
            await clientSocket.SendAsync(Encoding.UTF8.GetBytes(initPayload), WebSocketMessageType.Text, true, CancellationToken.None);

            await ReciveLoop(clientSocket, service, board.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Got Exeption in WebSocket: {ex}");
        }
        finally
        {
            session.RemoveClient(clientSocket);

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

    private async Task ReciveLoop(WebSocket clientSocket, BoardService service, string boardId)
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
            await HandleMessage(clientSocket, service, message, boardId);
        }
    }

    private async Task HandleMessage(WebSocket clientSocket, BoardService service, string messageString, string boardId)
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
                await HandleAddStroke(clientSocket, service, message, boardId);
                break;

            case "addPointToStroke":
                await HandleAddPointToStroke(clientSocket, service, message, boardId);
                break;

            case "undo":
                await HandleUndo(clientSocket, service, message, boardId);
                break;

            case "clearBoard":
                await HandleClearBoard(clientSocket, service, message, boardId);
                break;

            default:
                BoardMessage errorMessage = new() { Type = "error", ErrorMessage = "Got a unknown type needs to be addStroke or addPointToStroke" };
                var errorPayload = JsonSerializer.Serialize(errorMessage, Config.Json.Options);
                await clientSocket.SendAsync(Encoding.UTF8.GetBytes(errorPayload), WebSocketMessageType.Text, true, CancellationToken.None);
                break;
        }
    }

    private Task HandleAddStroke(WebSocket clientSocket, BoardService service, BoardMessage message, string boardId)
    { 
        if (message.Stroke == null) 
        {
            return SendErrorMessageToClient(
                clientSocket,
                "Type addStroke needs a stroke value but didn't get one.");
        }

        service.AddStroke(boardId, message.Stroke.ToEntity(boardId));

        return BroadCastToOthers(clientSocket, message, boardId);
    }

    private Task HandleAddPointToStroke(WebSocket clientSocket, BoardService service, BoardMessage message, string boardId)
    {
        if (message.StrokeId == null || message.Point == null)
        {
            return SendErrorMessageToClient(
                clientSocket,
                "Type addPointToStroke needs a strokeId and point value but didn't get them.");
        }

        service.AddPointToStroke(message.StrokeId, message.Point.ToEntity(message.StrokeId));

        return BroadCastToOthers(clientSocket, message, boardId);
    }

    private Task HandleUndo(WebSocket clientSocket, BoardService service, BoardMessage message, string boardId)
    {
        if (message.UserId == null)
        {
            return SendErrorMessageToClient(
                clientSocket, 
                "Type undo needs a userId but didn't get one.");
        }

        service.RemoveUsersLastStroke(message.UserId);

        return BroadCastToOthers(clientSocket, message, boardId);
    }

    private Task HandleClearBoard(WebSocket clientSocket, BoardService service, BoardMessage message, string boardId)
    {
        service.ClearBoard(boardId);

        return BroadCastToOthers(clientSocket, message, boardId);
    }

    private async Task BroadCastToOthers(WebSocket sender, BoardMessage message, string boardId)
    {
        var payload = JsonSerializer.Serialize(message, Config.Json.Options);
        var bytes = Encoding.UTF8.GetBytes(payload);

        var clients = _manager.GetOrCreateSession(boardId).Clients;

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
