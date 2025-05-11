using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class TerrainArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly List<string> _stringIdLookup;

    public override bool IsNecessary => false;

    public TerrainArchetypeDataSectionReader(List<string> stringIdLookup)
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        styleData.TerrainArchetypeData.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainArchetypeDatum = ReadNextTerrainArchetypeData(styleData.Identifier, rawFileData);
            styleData.TerrainArchetypeData.Add(newTerrainArchetypeDatum.PieceName, newTerrainArchetypeDatum);
        }
    }

    private TerrainArchetypeData ReadNextTerrainArchetypeData(
        StyleIdentifier styleName,
        RawStyleFileDataReader rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int pieceId = rawFileData.Read16BitUnsignedInteger();

        uint terrainArchetypeDataByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.DecodeTerrainArchetypeDataByte(
            terrainArchetypeDataByte,
            out var isSteel,
            out var resizeType);

        int defaultWidth = 0;
        int defaultHeight = 0;

        int nineSliceBottom = 0;
        int nineSliceLeft = 0;
        int nineSliceTop = 0;
        int nineSliceRight = 0;

        if (resizeType.CanResizeHorizontally())
        {
            defaultWidth = rawFileData.Read8BitUnsignedInteger();

            if (defaultWidth > 0)
            {
                nineSliceLeft = rawFileData.Read8BitUnsignedInteger();
                nineSliceRight = rawFileData.Read8BitUnsignedInteger();
            }
        }

        if (resizeType.CanResizeVertically())
        {
            defaultHeight = rawFileData.Read8BitUnsignedInteger();

            if (defaultHeight > 0)
            {
                nineSliceBottom = rawFileData.Read8BitUnsignedInteger();
                nineSliceTop = rawFileData.Read8BitUnsignedInteger();
            }
        }

        FileReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "terrain archetype data section");

        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            StyleName = styleName,
            PieceName = new PieceIdentifier(_stringIdLookup[pieceId]),

            IsSteel = isSteel,
            ResizeType = resizeType,

            DefaultWidth = defaultWidth,
            DefaultHeight = defaultHeight,

            NineSliceBottom = nineSliceBottom,
            NineSliceLeft = nineSliceLeft,
            NineSliceTop = nineSliceTop,
            NineSliceRight = nineSliceRight
        };

        return newTerrainArchetypeData;
    }
}
