using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class TerrainArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public TerrainArchetypeDataSectionReader(FileReaderStringIdLookup stringIdLookup)
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
            styleData.TerrainArchetypeData.Add(newTerrainArchetypeDatum.PieceIdentifier, newTerrainArchetypeDatum);
        }
    }

    private TerrainArchetypeData ReadNextTerrainArchetypeData(
        StyleIdentifier styleName,
        RawStyleFileDataReader rawFileData)
    {
        int pieceId = rawFileData.Read16BitUnsignedInteger();
        int nameId = rawFileData.Read16BitUnsignedInteger();

        uint terrainArchetypeDataByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.DecodeTerrainArchetypeDataByte(
            terrainArchetypeDataByte,
            out var isSteel,
            out var resizeType);

        var nineSliceData = ReadNineSliceData(rawFileData, resizeType, out var defaultSize);

        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            StyleIdentifier = styleName,
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),
            Name = _stringIdLookup[nameId],

            IsSteel = isSteel,
            ResizeType = resizeType,

            DefaultSize = defaultSize,

            NineSliceData = nineSliceData
        };

        return newTerrainArchetypeData;
    }

    private static RectangularRegion ReadNineSliceData(
        RawStyleFileDataReader rawFileData,
        ResizeType resizeType,
        out Size defaultSize)
    {
        int defaultWidth = 0;
        int defaultHeight = 0;

        int nineSliceLeft = 0;
        int nineSliceWidth = 0;
        int nineSliceTop = 0;
        int nineSliceHeight = 0;

        if (resizeType.CanResizeHorizontally())
        {
            defaultWidth = rawFileData.Read16BitUnsignedInteger();

            if (defaultWidth > 0)
            {
                nineSliceLeft = rawFileData.Read16BitUnsignedInteger();
                nineSliceWidth = rawFileData.Read16BitUnsignedInteger();

                FileReadingException.ReaderAssert(nineSliceLeft >= 0, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceWidth >= 1, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceLeft + nineSliceWidth <= defaultWidth, "Invalid nine slice definition!");
            }
        }

        if (resizeType.CanResizeVertically())
        {
            defaultHeight = rawFileData.Read16BitUnsignedInteger();

            if (defaultHeight > 0)
            {
                nineSliceTop = rawFileData.Read16BitUnsignedInteger();
                nineSliceHeight = rawFileData.Read16BitUnsignedInteger();

                FileReadingException.ReaderAssert(nineSliceTop >= 0, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceHeight >= 1, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceTop + nineSliceHeight <= defaultHeight, "Invalid nine slice definition!");
            }
        }

        defaultSize = new Size(defaultWidth, defaultHeight);
        var p = new Point(nineSliceLeft, nineSliceTop);
        var s = new Size(nineSliceWidth, nineSliceHeight);
        return new RectangularRegion(p, s);
    }
}
