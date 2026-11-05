using System;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class ClockEnvironment : GameEnvironmentBase, IDrawable, IUpdateable
{
    private readonly Nums _nums;
    private int _lastSecond = -1;

    public Texture ClockTexture { get; private set; } = new Texture(33, 4);
    public Texture[] ClockNums { get; private set; } = new Texture[6];

    public ClockEnvironment()
    {
        _nums = new Nums();
        for (int i = 0; i < _nums.NumberTextures.Length; i++)
        {
            _nums.NumberTextures[i].IgnoreSet.Clear();
        }

        for (int i = 0; i < ClockNums.Length; i++)
        {
            ClockNums[i] = new Texture(4, 4);
            ClockNums[i].IgnoreSet.Clear();
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

        if (time.Second != _lastSecond)
        {
            _lastSecond = time.Second;
            UpdateClock(time.Hour, time.Minute, time.Second);
        }
    }

    public void UpdateClock(int hours, int minutes, int seconds)
    {
        CopyNumberBuffer(ClockNums[0], _nums.NumberTextures[hours / 10]);
        CopyNumberBuffer(ClockNums[1], _nums.NumberTextures[hours % 10]);
        CopyNumberBuffer(ClockNums[2], _nums.NumberTextures[minutes / 10]);
        CopyNumberBuffer(ClockNums[3], _nums.NumberTextures[minutes % 10]);
        CopyNumberBuffer(ClockNums[4], _nums.NumberTextures[seconds / 10]);
        CopyNumberBuffer(ClockNums[5], _nums.NumberTextures[seconds % 10]);
    }

    private static void CopyNumberBuffer(Texture dest, Texture src)
    {
        src.Buffer.AsSpan().CopyTo(dest.Buffer.AsSpan());
        dest.ForegroundColor = src.ForegroundColor;
        dest.BackgroundColor = src.BackgroundColor;
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        Style.ToPositionByWidth(ClockTexture, context.GetCanvasWidth(), Position.Center);
        ClockTexture.Y = 6;

        ClockTexture = ClockTexture.Join(_nums.DoublePoints, 10, 0);
        ClockTexture = ClockTexture.Join(_nums.DoublePoints, 22, 0);

        int[] posX = [0, 5, 12, 17, 24, 29];
        for (int i = 0; i < ClockNums.Length; i++)
        {
            ClockNums[i].X = posX[i] + ClockTexture.X;
            ClockNums[i].Y = ClockTexture.Y;
        }

        var now = DateTime.Now;
        _lastSecond = now.Second;
        UpdateClock(now.Hour, now.Minute, now.Second);
    }
}