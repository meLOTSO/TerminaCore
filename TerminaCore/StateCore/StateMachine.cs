using TerminaCore.StateCore.Delegates;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace TerminaCore.StateCore;

// Главный аппарат машины состояний, инкапуслрующий в себе словарь состояний, смену состояний, события при смене состояния
public class StateMachine<TKey, TValue> where TKey : notnull
{
    private TKey? _currentState = default; // текущее состояние
    private TKey? _nextState;
    private bool _hasPendingTransition = false;
    private ICollection<TValue> _currentValues = []; // список значений текущего состояния

    // События смены состояния
    protected event StateChangedDelegate<TKey, TValue>? StateChangedEvent = null;

    public FrozenDictionary<TKey, FrozenSet<TValue>> StateValueData { get; protected set; } = FrozenDictionary<TKey, FrozenSet<TValue>>.Empty;
    public StateBag<TKey, TValue> StateBag { get; private set; }
    // Предыдущее состояние
    public TKey? PreviousState { get; private set; }
    // Текущее состояние
    public TKey? CurrentState
    {
        get => _currentState; // получаем текущее состояние
        set
        {
            SetState(value!); // при изменении вызываем метод по смене состояния
        }
    }

    // Список значений текущего состояния
    public ICollection<TValue> CurrentValues
    {
        get => _currentValues; // получаем список значений текущего состояния
        private set
        {
            _currentValues = value; // при изменении обновляем список из значений текущего состояния
        }
    }

    // Инкапсулирует текущие состояние/значение и метод по смене состояния
    public StateContext<TKey, TValue> StateContext { get; private set; }

    // Добавляем словарь состояний-списка_значений
    public StateMachine(IDictionary<TKey, List<TValue>> stateDictionary) : this()
    {
        StateValueData = stateDictionary.ToFrozenDictionary(
            pair => pair.Key,
            pair => pair.Value.ToFrozenSet()
        );
    }

    // Регистрируем обработчик смены состояний для StateContext, связываем с методом SetState объекта StateMachine
    public StateMachine()
    {
        StateContext = new StateContext<TKey, TValue>(this);
    }

    // Метод для изменения состояния
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
        CurrentValues = StateValueData.GetValueOrDefault(toState) ?? [];

        StateBag = new StateBag<TKey, TValue>
        {
            PreviousState = PreviousState,
            CurrentState = _currentState,
            CurrentValues = CurrentValues
        };

        StateChangedEvent?.Invoke(StateContext);
        return StateBag;
    }

    // Метод для добавления обработки смены состояния
    public void AddOnStateChangeHandler(StateChangedDelegate<TKey, TValue> onStateChangeHandler)
    {
        StateChangedEvent += onStateChangeHandler;
    }
    // Метод для удаления обработки смены состояния
    public void RemoveOnStateChangeHandler(StateChangedDelegate<TKey, TValue> onStateChangeHandler)
    {
        StateChangedEvent -= onStateChangeHandler;
    }
}
