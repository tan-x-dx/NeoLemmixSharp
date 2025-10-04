using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class TerrainArchetypeDataSectionWriter : StyleDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public TerrainArchetypeDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        var numberOfNonTrivialArchetypeData = 0;

        foreach (var kvp in styleData.TerrainArchetypeDataLookup)
        {
            var terrainArchetypeData = kvp.Value;

            if (terrainArchetypeData.IsNonTrivial())
                numberOfNonTrivialArchetypeData++;
        }

        return (ushort)numberOfNonTrivialArchetypeData;
    }

    public override void WriteSection(RawStyleFileDataWriter writer, StyleData styleData)
    {
        foreach (var kvp in styleData.TerrainArchetypeDataLookup)
        {
            var terrainArchetypeData = kvp.Value;

            if (terrainArchetypeData.IsNonTrivial())
                WriteTerrainArchetypeData(writer, terrainArchetypeData);
        }
    }

    private void WriteTerrainArchetypeData(RawStyleFileDataWriter writer, TerrainArchetypeData terrainArchetypeData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(terrainArchetypeData.PieceIdentifier));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(terrainArchetypeData.Name));

        var resizeType = terrainArchetypeData.ResizeType;
        writer.Write8BitUnsignedInteger((byte)ReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainArchetypeData.IsSteel, resizeType));

        if (resizeType.CanResizeHorizontally())
        {
            writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.DefaultSize.W);

            if (terrainArchetypeData.DefaultSize.W > 0)
            {
                writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.NineSliceData.X);
                writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.NineSliceData.W);
            }
        }

        if (resizeType.CanResizeVertically())
        {
            writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.DefaultSize.H);

            if (terrainArchetypeData.DefaultSize.H > 0)
            {
                writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.NineSliceData.Y);
                writer.Write16BitUnsignedInteger((ushort)terrainArchetypeData.NineSliceData.H);
            }
        }
    }
}
