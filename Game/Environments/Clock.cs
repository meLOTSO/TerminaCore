using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class Clock : GameEnvironmentBase, IDrawable, IUpdateable
{
    private readonly Nums _nums;

    public Texture[] ClockNums { get; private set; } = new Texture[6];
    public Texture ClockTexture { get; private set; } = new Texture(4, 4 * 6 + 3 + 2 * 3);

    public Clock()
    {
        _nums = new Nums();
        for (int i = 0; i < _nums.NumberTextures.Length; i++)
        {
            _nums.NumberTextures[i].IgnoreSet.Clear();
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(ClockTexture);
        for (int i = 0; i < ClockNums.Length; i++)
        {
            context.Draw(ClockNums[i]);
        }
    }

    public void Update(UpdateContext context)
    {
        var time = DateTime.Now;
        UpdateClock(time.Hour, time.Minute, time.Second);
    }

    public void UpdateClock(int hours, int minutes, int seconds)
    {
        ClockNums[0] = ClockNums[0].Join(_nums.NumberTextures[hours / 10]);
        ClockNums[1] = ClockNums[1].Join(_nums.NumberTextures[hours % 10]);
        ClockNums[2] = ClockNums[2].Join(_nums.NumberTextures[minutes / 10]);
        ClockNums[3] = ClockNums[3].Join(_nums.NumberTextures[minutes % 10]);
        ClockNums[4] = ClockNums[4].Join(_nums.NumberTextures[seconds / 10]);
        ClockNums[5] = ClockNums[5].Join(_nums.NumberTextures[seconds % 10]);
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);
        Style.ToPositionByWidth(ClockTexture, context.GetCanvasWidth(), Position.Center);
        ClockTexture.Y = 6;
        ClockTexture = ClockTexture.Join(_nums.DoublePoints, 10, 0);
        ClockTexture = ClockTexture.Join(_nums.DoublePoints, 22, 0);

        int[] posX = new int[] { 0, 5, 12, 17, 24, 29 };
        for (int i = 0; i < ClockNums.Length; i++)
        {
            ClockNums[i] = new Texture(4, 4);
            ClockNums[i].X = posX[i] + ClockTexture.X;
            ClockNums[i].IgnoreSet.Clear();
            ClockNums[i].Y = ClockTexture.Y;
        }
        _nums.DoublePoints.ForegroundColor = ConsoleColor.Black;
    }
}