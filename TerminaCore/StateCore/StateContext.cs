namespace TerminaCore.StateCore;

public class StateContext<TKey, TValue> where TKey : notnull
{
    private readonly StateMachine<TKey, TValue> _stateMachine;

    public TKey? PreviousState => _stateMachine.PreviousState;
    public TKey? CurrentState => _stateMachine.CurrentState;
    public ICollection<TValue> CurrentValues => _stateMachine.CurrentValues;


    public StateContext(StateMachine<TKey, TValue> stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void SetState(TKey toState, bool usePendingState = false)
    {
        _stateMachine.SetState(toState, usePendingState);
    }
}
