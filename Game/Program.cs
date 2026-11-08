using Game.Environments;
using Game.Environments.DisplayInteractive;
using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

var builder = new GameApplicationBuilder();

builder.Entities.UseEntity<BankCore>();

var canvas = builder.Services.AddASCIIGraphic(120, 30, backgroundColor: ConsoleColor.DarkRed);
builder.Services.AddDOMModel(services => services.GetServiceRequired<Canvas>());
builder.Services.AddStateMachine(builder =>
{
    builder.AddValueForStates(GameScene.FromGameEnvironment<CursorEnvironment>(), "load");
    builder.AddValuesForState("menu",
        GameScene.FromGameEnvironment<MenuEnvironment>(),
        GameScene.FromGameEnvironment<ClockEnvironment>());
    builder.AddValuesForState("new_game",
        GameScene.FromGameEnvironment<NewGameEnvironment>());
});

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();
app.UseDOMModel();

app.SetState("menu");

app.Run();

