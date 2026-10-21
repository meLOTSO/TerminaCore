using StateCore.Delegates;

namespace StateCore;

public class StateMachineContainer<TKey, TValue> where TKey : notnull
{
    protected event SetStateDelegate<TKey>? OnSetState = null;

    public StateMachineContainer<TKey, TValue> AddStateMachine(StateMachine<TKey, TValue> stateMachine)
    {
        OnSetState += (state) => stateMachine.SetState(state);
        return this;
    }

    public void SetState(TKey state) => OnSetState?.Invoke(state);
}
