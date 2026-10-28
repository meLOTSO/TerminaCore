using TerminaCore.GameCore;
using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Enums;
using TerminaCore.GameCore.Extensions;

var builder = new GameApplicationBuilder();

builder.Entities.UseEntity<Clock>();

var canvas = builder.Services.AddASCIIGraphic(100, 28, backgroundColor: ConsoleColor.DarkRed);
builder.Services.AddDOMModel(canvas);
builder.Services.AddStateMachine(builder =>
{
    builder.AddValuesForState("menu",
        GameScene.FromGameEnvironment<Menu>(),
        GameScene.FromGameEnvironment<Clock>());
    builder.AddValueForState("new_game",
        GameScene.FromGameEnvironment<Game>());
});

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();
app.UseDOMModel();

app.SetState("menu");

app.Run();