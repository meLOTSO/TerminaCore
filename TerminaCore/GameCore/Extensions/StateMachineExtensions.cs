using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Delegates;
using TerminaCore.GameCore.Contexts;
using TerminaCore.ServiceCore.Abstract;
using TerminaCore.StateCore;
using TerminaCore.StateCore.Builder;
using TerminaCore.Debug;
using TerminaCore.GameCore.Abstract;
using TerminaCore.GameCore.Enums;

namespace TerminaCore.GameCore.Extensions;

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

        Dictionary<Type, object> environments = new();
        foreach (var stateValuePair in stateMachine.StateValueData)
        {
            foreach (IGameScene gameScene in stateValuePair.Value)
            {
                Type type = gameScene.FactoryItem.Key;

                if (gameScene.FactoryItem.Value is null)
                    throw new InvalidOperationException($"В сцене не зарегистрирована фабрика окружения (FactoryItem пуст).");

                if (!environments.TryGetValue(type, out object? environment))
                {
                    environment = gameScene.FactoryItem.Value.Invoke();
                    environments.Add(type, environment);

                    FileDebug.WriteLine($"UseStateMachine: TryReleaseEntity({type.Name}, {environment})");
                    gameApplication.Entities.TryReleaseEntity(type, environment);
                }

                if (gameScene.GameFunctions.HasFlag(GameFunctions.Update) && environment is IUpdateable u) gameScene.Update = u.Update;
                if (gameScene.GameFunctions.HasFlag(GameFunctions.OnKeyPress) && environment is IControl c) gameScene.OnKeyPress = c.OnKeyPress;
                if (gameScene.GameFunctions.HasFlag(GameFunctions.Draw) && environment is IDrawable d) gameScene.Draw = d.Draw;
            }
        }
        foreach (var environment in environments.Values)
        {
            if (environment is IInitializableEnvironment initializableEnvironment)
                initializableEnvironment.Initialize(gameApplication.GameContext);
        }

        stateMachine.AddOnStateChangeHandler(stateContext =>
        {
            // освобождение ресурсов при смене состояния в окружениях, реализующих IExitEnvironment 
            if (stateContext.PreviousState is not null
                && stateMachine.StateValueData.TryGetValue(stateContext.PreviousState, out var previousScenes))
            {
                foreach (var scene in previousScenes)
                {
                    Type type = scene.FactoryItem.Key;
                    if (environments.TryGetValue(type, out var env))
                    {
                        if (env is IExitEnvironment exitEnvironment)
                            exitEnvironment.OnExit();
                    }
                }
            }

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

        gameApplication.AfterStop += () =>
        {
            foreach (var env in environments.Values)
            {
                if (env is IDisposable disposable)
                    disposable.Dispose();
            }
            environments.Clear();
        };
    }

    public static StateContext<string, IGameScene>? GetStateContext(
        this TerminaCore.ServiceCore.Abstract.IServiceProvider serviceProvider
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