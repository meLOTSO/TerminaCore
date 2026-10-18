using GameCore.Abstract.GameBasicFunctional;
using GameCore.Delegates;
using ServiceCore.Abstract;
using StateCore;
using StateCore.Builder;

namespace GameCore.Extensions;

public static class StateMachineExtensions
{
    public static void AddStateMachine(
        this IServiceCollectionBuilder services,
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
        this IServiceCollection serviceCollection
    )
    {
        var service = serviceCollection.GetService<StateMachine<string, IGameScene>>();
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