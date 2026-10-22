namespace StateCore.Delegates;

// Делегат, принимающий предыдущее состояние и текущее
public delegate void StateChangedDelegate<TKey, TValue>(StateContext<TKey, TValue> stateContext) where TKey : notnull;