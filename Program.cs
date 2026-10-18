// using GameCore;
// using GameCore.Extensions;
// using GameCore.Builder;

// var gameBuilder = new GameApplicationBuilder();

// gameBuilder.Services.AddStateMachine(smBuilder =>
// {
//     smBuilder.AddValueForState("menu", GameScene.FromEntity(new Entity()));
//     smBuilder.AddValueForState("setting", GameScene.FromEntity(new Setting()));
// });

// var game = gameBuilder.Build();

// game.UseStateMachine();

// game.SetState("menu");

// game.Run();

using Graphics;

Console.Clear();


var texture = new Texture(new[,]
{
    {' ', ' ', ' ', '3', ' ', ' '},
    {' ', ' ', '2', ' ', '4', ' '},
    {' ', '1', ' ', ' ', ' ', '5'},
    {'0', ' ', ' ', ' ', ' ', ' ' }
});



var t = new Texture(new List<string>
{
    "&&  &&",
    "*  ",
    "   99"
});

Console.SetCursorPosition(0, 0);

string path = @"C:\Users\MOLL\OneDrive\Документы\KAI\7 семестр\ТВППИТС (паттерны программирования)\lab1\code\Test\texture.txt";

var spr = new Texture(path);


Canvas canvas = new(20, 10);
canvas.DrawRange(texture, t, spr);

int p = 1;
while (true)
{
    canvas.Clear();
    canvas.Render();
    canvas.Display();
    spr.X += p;
    if (spr.X > 10) p = -1;
    else if (spr.X < -5) p = 1;
    Thread.Sleep(20);
}