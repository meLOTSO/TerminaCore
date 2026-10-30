using TerminaCore.Graphics;
using TerminaCore.ServiceCore.Abstract;
using TerminaCore.GameCore.Contexts;

namespace TerminaCore.GameCore.Extensions;

public static class GraphicExtensions
{
    public static Canvas AddASCIIGraphic(this IServiceCollection services,
        int width, int height, char backgroundChar = ' ',
        ConsoleColor foregroundColor = default, ConsoleColor backgroundColor = default)
    {
        if (width < 0 || height < 0)
            throw new ArgumentException("Ширина и высота окна должны иметь положительные значения.");

        Console.Clear();
        var canvas = new Canvas(width, height, backgroundChar, foregroundColor, backgroundColor);
        services.AddSingleton<Canvas>(canvas);
        return canvas;
    }

    public static GameApplication UseASCIIGraphic(this GameApplication gameApplication)
    {
        Canvas? canvas = gameApplication.Services.GetService<Canvas>() ?? throw new Exception("Сервис 'Canvas' не был добавлен. Воспользуйтесь AddASCIIGraphis.");

        gameApplication.BeforeDraw += canvas.Clear;
        gameApplication.AfterDraw += () => canvas.Render();
        gameApplication.AfterDraw += canvas.Display;
        gameApplication.AfterStop += () =>
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        };
        return gameApplication;
    }

    public static void Draw(this DrawContext context, Texture texture)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return;
        canvas.Draw(texture);
    }
    public static int GetCanvasWidth(this GameContext context)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return 0;
        return canvas.Width;
    }
    public static int GetCanvasHeight(this GameContext context)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return 0;
        return canvas.Height;
    }
    public static Canvas? GetCanvas(this GameContext context)
    {
        return context.Services.GetService<Canvas>();
    }
    public static Canvas GetCanvasRequired(this GameContext context)
    {
        return context.Services.GetServiceRequired<Canvas>();
    }
    public static int GetCanvasWidth(this DrawContext context)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return 0;
        return canvas.Width;
    }
    public static int GetCanvasHeight(this DrawContext context)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return 0;
        return canvas.Height;
    }
    public static Canvas? GetCanvas(this DrawContext context)
    {
        return context.Services.GetService<Canvas>();
    }
    public static Canvas GetCanvasRequired(this DrawContext context)
    {
        return context.Services.GetServiceRequired<Canvas>();
    }
}
