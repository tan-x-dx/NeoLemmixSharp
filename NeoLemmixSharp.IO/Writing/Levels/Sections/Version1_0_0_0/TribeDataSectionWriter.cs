using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class TribeDataSectionWriter : LevelDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public TribeDataSectionWriter(StringIdLookup stringIdLookup)
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

    private void WriteTribeIdentifierData(RawLevelFileDataWriter writer, TribeIdentifier tribeIdentifier)
    {
        writer.Write(_stringIdLookup.GetStringId(tribeIdentifier.StyleIdentifier.ToString()));
        writer.Write((byte)tribeIdentifier.ThemeTribeId);
    }
}
