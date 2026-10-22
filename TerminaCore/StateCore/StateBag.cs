using System.Collections.Generic;

namespace TerminaCore.StateCore;

public record struct StateBag<TKey, TValue>(TKey? PreviousState, TKey CurrentState, ICollection<TValue> CurrentValues);
