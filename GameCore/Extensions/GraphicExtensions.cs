using GameCore;
using Graphics;
using ServiceCore.Abstract;

public static class GraphicExtensions
{
    public static void AddASCIIGraphic(this IServiceCollectionBuilder services, int width, int height, char backgroundChar = ' ')
    {
        services.AddSingleton<Canvas>(() => new Canvas(width, height, backgroundChar));
    }

    public static void UseASCIIGraphic(this GameApplication gameApplication)
    {
        Canvas? canvas = gameApplication.Services.GetService<Canvas>();
        if (canvas is null) throw new Exception("Сервис 'Canvas' не был добавлен. Воспользуйтесь AddASCIIGraphis.");

        gameApplication.BeforeDraw += canvas.Clear;
        gameApplication.AfterDraw += canvas.Render;
        gameApplication.AfterDraw += canvas.Display;
    }

    public static void Draw(this DrawContext context, Texture texture)
    {
        Canvas? canvas = context.Services.GetService<Canvas>();
        if (canvas is null) return;
        canvas.Draw(texture);
    }
}
