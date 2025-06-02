using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class TerrainArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public TerrainArchetypeDataSectionReader(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
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
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        uint terrainArchetypeDataByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.DecodeTerrainArchetypeDataByte(
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

        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            StyleName = styleName,
            PieceName = new PieceIdentifier(_stringIdLookup[pieceId]),

            IsSteel = isSteel,
            ResizeType = resizeType,

            DefaultWidth = defaultWidth,
            DefaultHeight = defaultHeight,

            NineSliceData =  new NineSliceData(nineSliceBottom, nineSliceLeft, nineSliceTop, nineSliceRight)
        };

        return newTerrainArchetypeData;
    }
}
