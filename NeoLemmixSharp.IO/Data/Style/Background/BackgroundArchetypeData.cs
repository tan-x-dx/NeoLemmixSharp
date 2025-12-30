using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Background;

public sealed class BackgroundArchetypeData : IArchetypeData
{
    TextureType IArchetypeData.TextureType => TextureType.Background;

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required string Name { get; init; }

    RectangularRegion IArchetypeData.NineSliceData { get; }
    ResizeType IArchetypeData.ResizeType { get; }
    Size IArchetypeData.DefaultSize { get; }
}
