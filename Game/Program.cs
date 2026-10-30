using Game.Environments;
using TerminaCore.GameCore;
using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Enums;
using TerminaCore.GameCore.Extensions;

var builder = new GameApplicationBuilder();

builder.Entities.UseEntity<ClockEnvironment>();

var canvas = builder.Services.AddASCIIGraphic(100, 28, backgroundColor: ConsoleColor.DarkRed);
builder.Services.AddDOMModel(canvas);
builder.Services.AddStateMachine(builder =>
{
    builder.AddValueForStates(GameScene.FromGameEnvironment<CursorEnvironment>(), "new_game", "load");
    builder.AddValuesForState("menu",
        GameScene.FromGameEnvironment<MenuEnvironment>(),
        GameScene.FromGameEnvironment<ClockEnvironment>());
    builder.AddValuesForState("new_game",
        GameScene.FromGameEnvironment<GameEnvironment>());
});

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();
app.UseDOMModel();

app.SetState("menu");

app.Run();
