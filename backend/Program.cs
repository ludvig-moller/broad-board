using backend.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
app.UseMiddleware<BoardWebSocketHandler>();

app.Run();
