using backend;
using backend.Middleware;
using backend.Services;
using backend.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BoardDbContext>();
builder.Services.AddScoped<BoardService>();

builder.Services.AddSingleton<BoardManager>();

builder.Services.AddHostedService<BoardCleanupService>();

var app = builder.Build();

app.UseWebSockets();
app.UseMiddleware<BoardWebSocketHandler>();

app.Run();
