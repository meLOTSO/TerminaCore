using System.Collections.Generic;
using System.Linq;
using TerminaCore.StateCore.Abstract;

namespace TerminaCore.StateCore.Builder;

// Конфигуратор применяется для редактирования "mutable state machine", настройки которой применяются к StateMachine
// Конфигуратор - прослойка для настраиваемых данных StateMachine (к примеру StateDictionaryMutable), он наследует их и применяет к StateMachine
public class StateMachineBuilder<TKey, TValue> : IStateDictionaryMutable<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, List<TValue>> _stateValueData = new();

    // Добавить одно значение для нескольких состояний
    public void AddValueForStates(TValue value, params TKey[] states)
    {
        foreach (var state in states)
            AddValueForState(state, value);
    }
    // Добавить одно значения для одного состояния
    public void AddValueForState(TKey state, TValue value)
    {
        if (_stateValueData.TryGetValue(state, out var list))
            list.Add(value);
        else
            _stateValueData.Add(state, new List<TValue> { value });
    }
    // Добавить несколько значения для одного состояния
    public void AddValuesForState(TKey state, params TValue[] values)
    {
        if (_stateValueData.TryGetValue(state, out var list))
            list.AddRange(values);
        else
            _stateValueData.Add(state, values.ToList());
    }

    // Применяем настройки mutable state machine к переданному stateMachine 
    public StateMachine<TKey, TValue> Build()
    {
        return new StateMachine<TKey, TValue>(_stateValueData);
    }
}
