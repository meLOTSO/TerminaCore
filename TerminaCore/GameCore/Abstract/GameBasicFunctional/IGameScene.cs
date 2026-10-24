using System;
using System.Collections.Generic;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Delegates;
using TerminaCore.GameCore.Enums;

namespace TerminaCore.GameCore.Abstract.GameBasicFunctional;

public interface IGameScene
{
    public OnKeyPressDelegate? OnKeyPress { get; set; }
    public UpdateDelegate? Update { get; set; }
    public DrawDelegate? Draw { get; set; }
    public KeyValuePair<Type, Func<object>> FactoryItem { get; }
    public GameFunctions GameFunctions { get; set; }
}
