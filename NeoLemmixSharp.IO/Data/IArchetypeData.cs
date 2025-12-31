using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data;

public interface IArchetypeData
{
    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }
    string Name { get; }
    string TextureFilePath { get; }

    RectangularRegion NineSliceData { get; }
    ResizeType ResizeType { get; }
    Size DefaultSize { get; }

    TextureType TextureType { get; }
}
