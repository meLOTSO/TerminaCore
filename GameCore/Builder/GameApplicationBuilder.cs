using GameCore.Abstract.GameBasicFunctional;
using GameCore.Delegates;
using ServiceCore;
using ServiceCore.Abstract;

namespace GameCore.Builder;

public class GameApplicationBuilder : GameBase, IGameFunctionalMutable
{
    private readonly ServiceCollection _services = new();

    public IServiceCollectionBuilder Services => _services;
    public List<UpdateDelegate> Updates { get => UpdateCollection; set => UpdateCollection = value; }
    public List<OnKeyPressDelegate> OnKeyPresses { get => OnKeyPressCollection; set => OnKeyPressCollection = value; }
    public List<DrawDelegate> Draws { get => DrawCollection; set => DrawCollection = value; }

    public void Entity<T>(T entity) where T : class, IEntityFunc
    {
        if (entity is IUpdateable u) UpdateCollection.Add(u.Update);
        if (entity is IControl c) OnKeyPressCollection.Add(c.OnKeyPress);
        if (entity is IDrawable d) DrawCollection.Add(d.Draw);
    }

    public GameApplication Build()
        => new GameApplication(this)
        {
            Services = _services
        };
}