using TerminaCore.StateCore.Abstract;

namespace TerminaCore.StateCore.Builder;

public class StateMachineBuilder<TKey, TValue> : IStateDictionaryMutable<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, List<TValue>> _stateValueData = new();

    public void AddValueForStates(TValue value, params TKey[] states)
    {
        foreach (var state in states)
            AddValueForState(state, value);
    }

    public void AddValueForState(TKey state, TValue value)
    {
        if (_stateValueData.TryGetValue(state, out var list))
            list.Add(value);
        else
            _stateValueData.Add(state, new List<TValue> { value });
    }

    public void AddValuesForState(TKey state, params TValue[] values)
    {
        if (_stateValueData.TryGetValue(state, out var list))
            list.AddRange(values);
        else
            _stateValueData.Add(state, values.ToList());
    }

    public StateMachine<TKey, TValue> Build()
    {
        return new StateMachine<TKey, TValue>(_stateValueData);
    }
}
