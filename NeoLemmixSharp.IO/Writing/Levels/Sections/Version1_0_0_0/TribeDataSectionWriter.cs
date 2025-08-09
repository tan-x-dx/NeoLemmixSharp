using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class TribeDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public TribeDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TribeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.TribeIdentifiers.Count;
    }

    public override void WriteSection(RawLevelFileDataWriter writer, LevelData levelData)
    {
        foreach (var tribeIdentifier in levelData.TribeIdentifiers)
        {
            WriteTribeIdentifierData(
                writer,
                tribeIdentifier);
        }
    }

    private void WriteTribeIdentifierData(RawLevelFileDataWriter writer, TribeStyleIdentifier tribeIdentifier)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(tribeIdentifier.StyleIdentifier));
        writer.Write8BitUnsignedInteger((byte)tribeIdentifier.ThemeTribeId);
    }
}
