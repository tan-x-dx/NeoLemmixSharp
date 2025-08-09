using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public interface IGadgetInstanceData
{
    GadgetIdentifier Identifier { get; }
    string OverrideName { get; }

    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }

    Point Position { get; }
    GadgetRenderMode GadgetRenderMode { get; }

    Orientation Orientation { get; }
    FacingDirection FacingDirection { get; }
}
