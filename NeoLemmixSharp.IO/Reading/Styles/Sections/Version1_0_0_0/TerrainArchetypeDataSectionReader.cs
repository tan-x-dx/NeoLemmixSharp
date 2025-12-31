using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
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

    public override void ReadSection(RawStyleFileDataReader reader, StyleData styleData, int numberOfItemsInSection)
    {
        styleData.TerrainArchetypeDataLookup.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainArchetypeDatum = ReadNextTerrainArchetypeData(styleData.Identifier, reader);
            styleData.TerrainArchetypeDataLookup.Add(newTerrainArchetypeDatum.PieceIdentifier, newTerrainArchetypeDatum);
        }
    }

    private TerrainArchetypeData ReadNextTerrainArchetypeData(
        StyleIdentifier styleIdentifier,
        RawStyleFileDataReader reader)
    {
        int pieceId = reader.Read16BitUnsignedInteger();
        int nameId = reader.Read16BitUnsignedInteger();

        uint terrainArchetypeDataByte = reader.Read8BitUnsignedInteger();
        ReadWriteHelpers.DecodeTerrainArchetypeDataByte(
            terrainArchetypeDataByte,
            out var isSteel,
            out var resizeType);

        var nineSliceData = ReadNineSliceData(reader, resizeType, out var defaultSize);

        var pieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]);
        var textureFilePath = ConstructTerrainPngFilePath(styleIdentifier, pieceIdentifier);

        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            StyleIdentifier = styleIdentifier,
            PieceIdentifier = pieceIdentifier,
            Name = _stringIdLookup[nameId],
            TextureFilePath = textureFilePath,

            NineSliceData = nineSliceData,
            ResizeType = resizeType,
            DefaultSize = defaultSize,

            IsSteel = isSteel
        };

        return newTerrainArchetypeData;
    }

    private static string ConstructTerrainPngFilePath(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier)
    {
        var rootFilePath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            styleIdentifier.ToString(),
            DefaultFileExtensions.TerrainFolderName,
            pieceIdentifier.ToString());

        return RootDirectoryManager.GetCorrespondingImageFile(rootFilePath);
    }

    private static RectangularRegion ReadNineSliceData(
        RawStyleFileDataReader reader,
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
            defaultWidth = reader.Read16BitUnsignedInteger();

            if (defaultWidth > 0)
            {
                nineSliceLeft = reader.Read16BitUnsignedInteger();
                nineSliceWidth = reader.Read16BitUnsignedInteger();

                FileReadingException.ReaderAssert(nineSliceLeft >= 0, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceWidth >= 1, "Invalid nine slice definition!");
                FileReadingException.ReaderAssert(nineSliceLeft + nineSliceWidth <= defaultWidth, "Invalid nine slice definition!");
            }
        }

        if (resizeType.CanResizeVertically())
        {
            defaultHeight = reader.Read16BitUnsignedInteger();

            if (defaultHeight > 0)
            {
                nineSliceTop = reader.Read16BitUnsignedInteger();
                nineSliceHeight = reader.Read16BitUnsignedInteger();

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
