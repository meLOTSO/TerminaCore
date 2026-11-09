using Game.Display;
using Game.Display.Enums;
using Game.Textures;
using Game.Tools;
using Game.Tools.Abstract;
using TerminaCore.DOMCore;
using TerminaCore.DOMCore.Enums;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class NewGameEnvironment : GameEnvironmentBase, IDrawable, IControl, IUpdateable
{
    private Texture _bg = null!;
    private Texture _map = null!;
    private Texture _labelExit = new Texture("[Escape] to exit");
    private List<IndexatorBag<IControl>> _controls = [];

    public InputText Input { get; } = new("ИМЯ:", 16);
    public Option Option { get; } = new("ВЫБРАТЬ ЦИВИЛИЗАЦИЮ");
    public Indexator<IndexatorBag<IControl>> TabIndexator { get; }

    public NewGameEnvironment()
    {
        TabIndexator = new(_controls);

        _controls.Add(new IndexatorBag<IControl>(Input,
            (s) => Input.ShowBlick = true,
            (s) => Input.ShowBlick = false));
        _controls.Add(new IndexatorBag<IControl>(Option,
            (s) =>
            {
                Option.LabelTexture.ForegroundColor = ConsoleColor.Gray;
                Option.LabelTexture.BackgroundColor = ConsoleColor.Black;
            },
            (s) =>
            {
                Option.LabelTexture.ForegroundColor = Option.LabelNonActiveColor;
                Option.LabelTexture.BackgroundColor = Option.LabelNonActiveBgColor;
            }));

        TabIndexator.OnIndexIn += (tag) => tag.ApplyOnIndexIn();
        TabIndexator.OnIndexOut += (tag) => tag.ApplyOnIndexOut();

        Input.InputTexture.ForegroundColor = ConsoleColor.DarkRed;
        Input.DisableCharColor = ConsoleColor.DarkRed;

        Option.AddSelect("Люди");
        Option.AddSelect("Эльфы");
        Option.AddSelect("Орки");
        Option.AddSelect("Вампиры");
        Option.MaxSelectedSelects = 1;

        Option.SetColorsForLabel(false, ConsoleColor.Gray, ConsoleColor.DarkGray);
        Option.SetColorsForLabel(true, ConsoleColor.Gray, ConsoleColor.DarkGray);
        Option.SetColorsForSelected(false, ConsoleColor.DarkGray, null);
        Option.SetColorsForSelected(true, ConsoleColor.DarkRed, null);
        Option.SetColorsForFocusSelect(true, ConsoleColor.Red, null);

        Option.Close();
    }

    public void Update(UpdateContext context)
    {
        Input.Update(context);
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Escape && !Option.IsActive)
        {
            GameContext.SetState("menu");
        }
        else if (controlContext.Key == ConsoleKey.Tab)
        {
            TabIndexator.TryNextIndex();
        }
        TabIndexator.GetCurrentValueOrDefault()?.Value.OnKeyPress(controlContext);
    }

    public void Draw(DrawContext context)
    {
        context.Draw(_bg);
        context.Draw(_map);
        Input.Draw(context);
        Option.Draw(context);
        context.Draw(_labelExit);
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        var canvas = context.GetCanvasRequired();

        _bg = new Texture(canvas.Width, canvas.Height, backgroundColor: ConsoleColor.Gray) { IgnoreSet = [] };
        _map = MapTextures.GetGround(canvas.Width, canvas.Height, '.', ConsoleColor.DarkGray, null, 800, Game.Enums.RenderType.Stack);
        _labelExit.X = 1;
        _labelExit.Y = _bg.Height - 2;

        var rootModel = new ModelNode(_bg);
        var inputModel = new ModelNode(Input.InputTexture);
        var optionModel = new ModelNode(Option.LabelTexture);

        rootModel.AddChild(inputModel);
        rootModel.AddChild(optionModel);

        rootModel.ApplyStyle(style =>
        {
            style.Display = Display.Flex;
            style.FlexPath = FlexPath.FlexColumn;
            style.FlexPosition = FlexPosition.Center;
            style.JustifyContent = FlexPosition.Center;
        });

        inputModel.ApplyStyle(style =>
        {
            style.Y = -4;
        });

        rootModel.UpdateStyleBFS();

        Input.InputMargin = 0;
        Input.SetLabelPosition(PositionOverEntity.TopCenter);
    }
}
