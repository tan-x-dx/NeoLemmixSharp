using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetArchetypeData
{
    GadgetType GadgetType { get; }

    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }
    GadgetName GadgetName { get; }

    Size BaseSpriteSize { get; }
}
