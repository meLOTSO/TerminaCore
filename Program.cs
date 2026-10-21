using GameCore;
using GameCore.Extensions;
using GameCore.Builder;
using System.Diagnostics;

var gameBuilder = new GameApplicationBuilder();

gameBuilder.Entities.UseEntity<Game>();

gameBuilder.Services.AddStateMachine(builder =>
{
    builder.AddValueForState("game", GameScene.FromGameEnvironment<Game>());
    builder.AddValueForState("menu", GameScene.FromGameEnvironment<Entity>());
    builder.AddValueForState("setting", GameScene.FromGameEnvironment<Setting>());
});
gameBuilder.Services.AddASCIIGraphic(40, 15);


var game = gameBuilder.Build();

game.UseStateMachine();
game.UseASCIIGraphic();

game.SetState("menu");

game.Run();
