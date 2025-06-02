using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class TerrainArchetypeDataSectionWriter : StyleDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public TerrainArchetypeDataSectionWriter(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        var numberOfNonTrivialArchetypeData = 0;

        foreach (var kvp in styleData.TerrainArchetypeData)
        {
            var terrainArchetypeData = kvp.Value;

            if (!terrainArchetypeData.IsTrivial())
                numberOfNonTrivialArchetypeData++;
        }

        return (ushort)numberOfNonTrivialArchetypeData;
    }

    public override void WriteSection(RawStyleFileDataWriter writer, StyleData styleData)
    {
        foreach (var kvp in styleData.TerrainArchetypeData)
        {
            var terrainArchetypeData = kvp.Value;

            if (!terrainArchetypeData.IsTrivial())
                WriteTerrainArchetypeData(writer, terrainArchetypeData);
        }
    }

    private void WriteTerrainArchetypeData(RawStyleFileDataWriter writer, TerrainArchetypeData terrainArchetypeData)
    {
        writer.Write(_stringIdLookup.GetStringId(terrainArchetypeData.PieceName.ToString()));

        var resizeType = terrainArchetypeData.ResizeType;
        writer.Write((byte)ReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainArchetypeData.IsSteel, resizeType));

        if (resizeType.CanResizeHorizontally())
        {
            writer.Write((byte)terrainArchetypeData.DefaultWidth);

            if (terrainArchetypeData.DefaultWidth > 0)
            {
                writer.Write((byte)terrainArchetypeData.NineSliceData.NineSliceLeft);
                writer.Write((byte)terrainArchetypeData.NineSliceData.NineSliceRight);
            }
        }

        if (resizeType.CanResizeVertically())
        {
            writer.Write((byte)terrainArchetypeData.DefaultHeight);

            if (terrainArchetypeData.DefaultHeight > 0)
            {
                writer.Write((byte)terrainArchetypeData.NineSliceData.NineSliceTop);
                writer.Write((byte)terrainArchetypeData.NineSliceData.NineSliceBottom);
            }
        }
    }
}
