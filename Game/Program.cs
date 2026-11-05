using Game.Environments;
using Game.Environments.DisplayInteractive;
using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

var builder = new GameApplicationBuilder();

builder.Entities.UseEntity<Coins>();

var canvas = builder.Services.AddASCIIGraphic(100, 28, backgroundColor: ConsoleColor.DarkRed);
builder.Services.AddDOMModel(services => services.GetServiceRequired<Canvas>());
builder.Services.AddStateMachine(builder =>
{
    builder.AddValueForStates(GameScene.FromGameEnvironment<CursorEnvironment>(), "new_game", "load");
    builder.AddValuesForState("menu",
        GameScene.FromGameEnvironment<MenuEnvironment>(),
        GameScene.FromGameEnvironment<ClockEnvironment>(),
        GameScene.FromGameEnvironment<CoinsEnvironment>());
    builder.AddValuesForState("new_game",
        GameScene.FromGameEnvironment<GameEnvironment>());
});

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();
app.UseDOMModel();

app.SetState("menu");

app.Run();
