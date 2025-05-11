using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

public sealed class GadgetArchetypeData
{
    public required string StyleName { get; init; }
    public required string PieceName { get; init; }

    public required GadgetType GadgetType { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }
    public required SpriteArchetypeData SpriteData { get; init; }
}
