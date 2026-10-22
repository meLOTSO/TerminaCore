using System.Collections.Generic;

namespace TerminaCore.StateCore;

// Инкапуслирует в себе контекст для работы с машиной состояний, доступный при работе с готовым аппаратом
public class StateContext<TKey, TValue> where TKey : notnull
{
    private readonly StateMachine<TKey, TValue> _stateMachine;

    // Предыдущее состояние
    public TKey? PreviousState => _stateMachine.PreviousState;
    // Текущее состояние
    public TKey? CurrentState => _stateMachine.CurrentState;
    // Текущий список значений состояния
    public ICollection<TValue> CurrentValues => _stateMachine.CurrentValues;


    // Важно добавить обработку смены состояния
    public StateContext(StateMachine<TKey, TValue> stateMachine)
    {
        _stateMachine = stateMachine;
    }
    // Смена состояния (вызывает обработчик, переданный через конструктор)
    public void SetState(TKey toState, bool usePendingState = false)
    {
        _stateMachine.SetState(toState, usePendingState);
    }
}
