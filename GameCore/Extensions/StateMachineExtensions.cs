using GameCore.Abstract.GameBasicFunctional;
using GameCore.Delegates;
using GameCore.Contexts;
using ServiceCore.Abstract;
using StateCore;
using StateCore.Builder;

namespace GameCore.Extensions;

public static class StateMachineExtensions
{
    public static void AddStateMachine(
        this IServiceCollection services,
        Action<StateMachineBuilder<string, IGameScene>> configure)
    {
        var builder = new StateMachineBuilder<string, IGameScene>();
        configure(builder);
        var sm = builder.Build();
        services.AddSingleton<StateMachine<string, IGameScene>>(sm);
    }

    public static void UseStateMachine(this GameApplication gameApplication)
    {
        var stateMachine = gameApplication.Services.GetService<StateMachine<string, IGameScene>>();
        if (stateMachine is null) return;

        gameApplication.PreFrame += stateMachine.ApplyPendingState;

        foreach (var stateValuePair in stateMachine.StateValueData)
        {
            foreach (IGameScene gameScene in stateValuePair.Value)
            {
                Type type = gameScene.FactoryItem.Key;
                var factory = gameScene.FactoryItem.Value;
                object entity = factory.Invoke(gameApplication.GameContext);

                if (entity is IUpdateable u) gameScene.Update = u.Update;
                if (entity is IControl c) gameScene.OnKeyPress = c.OnKeyPress;
                if (entity is IDrawable d) gameScene.Draw = d.Draw;

                FileDebug.WriteLine($"UseStateMachine: TryReleaseEntity({type.Name}, {entity})");
                gameApplication.Entities.TryReleaseEntity(type, entity);
            }
        }

        stateMachine.AddOnStateChangeHandler(stateContext =>
        {
            gameApplication.UpdateCollection.Clear();
            gameApplication.OnKeyPressCollection.Clear();
            gameApplication.DrawCollection.Clear();

            foreach (var scene in stateContext.CurrentValues)
            {
                if (scene.Update is UpdateDelegate u) gameApplication.UpdateCollection.Add(u);
                if (scene.OnKeyPress is OnKeyPressDelegate o) gameApplication.OnKeyPressCollection.Add(o);
                if (scene.Draw is DrawDelegate d) gameApplication.DrawCollection.Add(d);
            }
        });
    }

    public static StateContext<string, IGameScene>? GetStateContext(
        this ServiceCore.Abstract.IServiceProvider serviceProvider
    )
    {
        var service = serviceProvider.GetService<StateMachine<string, IGameScene>>();
        if (service is null) return null;
        return service.StateContext;
    }

    public static void SetState(
        this GameContext gameContext,
        string state
    )
    {
        var stateContext = gameContext.Services.GetStateContext();
        stateContext?.SetState(state, true);
    }
    public static void SetState(
        this GameApplication gameApplication,
        string state
    )
    {
        var stateContext = gameApplication.Services.GetStateContext();
        stateContext?.SetState(state, true);
    }
}