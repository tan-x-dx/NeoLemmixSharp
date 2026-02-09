using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Terrain;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public sealed class TerrainArchetypeData : ITerrainArchetypeData, IArchetypeData
{
    TextureType IArchetypeData.TextureType => TextureType.TerrainSprite;

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required string Name { get; init; }
    public required string TextureFilePath { get; init; }

    public required RectangularRegion NineSliceData { get; init; }
    public required ResizeType ResizeType { get; init; }
    public required Size DefaultSize { get; init; }
    public required bool IsSteel { get; init; }


    internal TerrainArchetypeData()
    {
    }

    internal bool IsNonTrivial()
    {
        return IsSteel ||
               ResizeType != ResizeType.None ||
               DefaultSize.W > 0 ||
               DefaultSize.H > 0;
    }

    internal static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       StyleIdentifier styleIdentifier,
       PieceIdentifier pieceIdentifier,
       string textureFilePath)
    {
        var texture = TextureCache.GetOrLoadTexture(styleIdentifier, pieceIdentifier, TextureType.TerrainSprite);

        return new()
        {
            StyleIdentifier = styleIdentifier,
            PieceIdentifier = pieceIdentifier,
            Name = pieceIdentifier.ToString(),
            TextureFilePath = textureFilePath,

            NineSliceData = default,
            ResizeType = ResizeType.None,
            DefaultSize = new Size(texture.Width, texture.Height),
            IsSteel = false
        };
    }
}
