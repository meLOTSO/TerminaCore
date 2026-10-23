using TerminaCore.GameCore;
using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Extensions;

var builder = new GameApplicationBuilder();

builder.Entities.UseEntity<Clock>();

builder.Services.AddASCIIGraphic(100, 28, backgroundColor: ConsoleColor.DarkRed);
builder.Services.AddStateMachine(builder =>
{
    builder.AddValuesForState("menu",
        GameScene.FromGameEnvironment<Menu>(),
        GameScene.FromGameEnvironment<Clock>());
});

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();

app.SetState("menu");

app.Run();