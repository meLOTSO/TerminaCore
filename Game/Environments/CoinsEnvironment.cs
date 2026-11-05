using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;

public class CoinsEnvironment : GameEnvironmentBase, IDrawable, IControl
{
    public Coins Coins { get; private set; } = null!;

    public void Draw(DrawContext context)
    {
        context.Draw(Coins.Texture);
        if (Coins.IsIntention)
            context.Draw(Coins.IntentionTexture);
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.P)
            Coins.IntentionToSummarize(1);
        else if (controlContext.Key == ConsoleKey.O)
            Coins.IntentionToSummarize(-1);
        else if (controlContext.Key == ConsoleKey.R)
            Coins.ApplyIntention();
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        Coins = context.Entities.GetEntityRequired<Coins>(true);

        Coins.Texture.IgnoreSet.Clear();
        Coins.Texture.ForegroundColor = ConsoleColor.Gray;
        Coins.Texture.BackgroundColor = ConsoleColor.Black;
        Coins.Texture.Y = 0;
        Style.ToPositionByWidth(Coins.Texture, context.GetCanvasWidth(), Position.Center);

        Coins.IntentionTexture.IgnoreSet.Clear();
        Coins.IntentionTexture.BackgroundColor = ConsoleColor.Black;
        Coins.IntentionTexture.X = Coins.Texture.X + Coins.Texture.Width;
        Coins.IntentionTexture.Y = Coins.Texture.Y;
    }
}