using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGadgetState : GadgetState
{
    public required HatchGadgetStateType Type { get; init; }

    public override GadgetRenderer Renderer { get; }
}
