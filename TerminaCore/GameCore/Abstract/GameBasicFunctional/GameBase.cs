using System.Collections.Generic;
using TerminaCore.GameCore.Delegates;

namespace TerminaCore.GameCore.Abstract.GameBasicFunctional;

public abstract class GameBase
{
    public List<UpdateDelegate> UpdateCollection { get; set; } = [];
    public List<OnKeyPressDelegate> OnKeyPressCollection { get; set; } = [];
    public List<DrawDelegate> DrawCollection { get; set; } = [];
}
