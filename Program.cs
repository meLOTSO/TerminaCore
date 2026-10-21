using GameCore;
using GameCore.Extensions;
using GameCore.Builder;
using System.Diagnostics;

var gameBuilder = new GameApplicationBuilder();

gameBuilder.Entities.UseEntity<Game>();

gameBuilder.Services.AddStateMachine(builder =>
{
    builder.AddValueForState("game", GameScene.FromEntity<Game>());
    builder.AddValueForState("menu", GameScene.FromEntity<Entity>());
    builder.AddValueForState("setting", GameScene.FromEntity<Setting>());
});
gameBuilder.Services.AddASCIIGraphic(40, 15);


var game = gameBuilder.Build();

game.UseStateMachine();
game.UseASCIIGraphic();

game.SetState("menu");

game.Run();
