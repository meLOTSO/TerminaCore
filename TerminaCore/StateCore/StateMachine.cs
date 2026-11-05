using TerminaCore.StateCore.Delegates;
using System.Collections.Frozen;

namespace TerminaCore.StateCore;

public class StateMachine<TKey, TValue> where TKey : notnull
{
    private TKey? _currentState = default;
    private ICollection<TValue> _currentValues = [];
    private TKey? _nextState;
    private bool _hasPendingTransition = false;

    protected event StateChangedDelegate<TKey, TValue>? StateChangedEvent = null;

    public FrozenDictionary<TKey, FrozenSet<TValue>> StateValueData { get; protected set; } = FrozenDictionary<TKey, FrozenSet<TValue>>.Empty;
    public StateBag<TKey, TValue> StateBag { get; private set; }
    // Предыдущее состояние
    public TKey? PreviousState { get; private set; }
    // Текущее состояние
    public TKey? CurrentState
    {
        get => _currentState;
        set
        {
            SetState(value!);
        }
    }

    public ICollection<TValue> CurrentValues
    {
        get => _currentValues;
    }

    public StateContext<TKey, TValue> StateContext { get; private set; }

    public StateMachine(IDictionary<TKey, List<TValue>> stateDictionary) : this()
    {
        StateValueData = stateDictionary.ToFrozenDictionary(
            pair => pair.Key,
            pair => pair.Value.ToFrozenSet()
        );
    }

    public StateMachine()
    {
        StateContext = new StateContext<TKey, TValue>(this);
    }

    public StateBag<TKey, TValue> SetState(TKey toState, bool usePendingState = false)
    {
        if (usePendingState)
        {
            _nextState = toState;
            _hasPendingTransition = true;
            return StateBag;
        }

        return TransitionTo(toState);
    }

    public void ApplyPendingState()
    {
        if (!_hasPendingTransition || _nextState is null) return;

        _hasPendingTransition = false;
        var targetState = _nextState;
        _nextState = default;
        TransitionTo(targetState);
    }

    public StateBag<TKey, TValue> TransitionTo(TKey toState)
    {
        PreviousState = _currentState;
        _currentState = toState;
        _currentValues = StateValueData.GetValueOrDefault(toState) ?? [];

        StateBag = new StateBag<TKey, TValue>
        {
            PreviousState = PreviousState,
            CurrentState = _currentState,
            CurrentValues = CurrentValues
        };

        StateChangedEvent?.Invoke(StateContext);
        return StateBag;
    }

    public void AddOnStateChangedHandler(StateChangedDelegate<TKey, TValue> onStateChangeHandler)
    {
        StateChangedEvent += onStateChangeHandler;
    }

    public void RemoveOnStateChangedHandler(StateChangedDelegate<TKey, TValue> onStateChangeHandler)
    {
        StateChangedEvent -= onStateChangeHandler;
    }
}
