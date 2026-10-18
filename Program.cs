using GameCore;
using GameCore.Extensions;
using GameCore.Builder;

var gameBuilder = new GameApplicationBuilder();

gameBuilder.Services.AddStateMachine(smBuilder =>
{
    smBuilder.AddValueForState("menu", GameScene.FromEntity(new Entity()));
    smBuilder.AddValueForState("setting", GameScene.FromEntity(new Setting()));
    smBuilder.AddValueForState("game", GameScene.FromEntity(new Game()));
});
gameBuilder.Services.AddASCIIGraphic(20, 10);

var game = gameBuilder.Build();

game.UseStateMachine();
game.UseASCIIGraphic();

game.SetState("menu");

Console.Clear();

game.Run();
