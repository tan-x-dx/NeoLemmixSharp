using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public interface IGadget
{
    int Id { get; }
    GadgetType Type { get; }
    Orientation Orientation { get; }
    LevelPosition LevelPosition { get; }

    IGadgetState CurrentState { get; }

    void Tick();
}