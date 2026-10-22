namespace TerminaCore.StateCore.Abstract;

public interface IStateDictionaryMutable<TKey, TValue> where TKey : notnull
{
    public void AddValueForStates(TValue value, params TKey[] states);
    public void AddValueForState(TKey state, TValue value);
    public void AddValuesForState(TKey state, params TValue[] values);
}