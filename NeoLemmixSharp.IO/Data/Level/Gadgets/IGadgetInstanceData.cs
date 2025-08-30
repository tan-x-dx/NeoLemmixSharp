using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public interface IGadgetInstanceData
{
    GadgetType GadgetType { get; }
    GadgetIdentifier Identifier { get; }
    GadgetName OverrideName { get; }

    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }

    Point Position { get; }
    GadgetRenderMode GadgetRenderMode { get; }

    Orientation Orientation { get; }
    FacingDirection FacingDirection { get; }
}
