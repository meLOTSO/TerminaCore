using TerminaCore.GameCore.Builder;
using TerminaCore.GameCore.Extensions;

var builder = new GameApplicationBuilder();

builder.Services.AddASCIIGraphic(40, 15);
builder.Services.AddStateMachine(builder =>
{
    // builder.AddValueForState();
});

// builder.Entities.UseEntity<>()

var app = builder.Build();

app.UseASCIIGraphic();
app.UseStateMachine();

app.Run();