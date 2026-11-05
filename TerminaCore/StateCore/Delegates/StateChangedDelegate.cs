namespace TerminaCore.StateCore.Delegates;

public delegate void StateChangedDelegate<TKey, TValue>(StateContext<TKey, TValue> stateContext) where TKey : notnull;