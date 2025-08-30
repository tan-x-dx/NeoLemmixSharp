using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetTriggerDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public GadgetTriggerDataSectionReader(FileReaderStringIdLookup stringIdLookup) : base(LevelFileSectionIdentifier.GadgetTriggerDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllGadgetLinkData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetLinkDatum = ReadNextGadgetLinkData(reader);
            levelData.AllGadgetLinkData.Add(newGadgetLinkDatum);
        }
    }

    private GadgetLinkData ReadNextGadgetLinkData(RawLevelFileDataReader reader)
    {
        int sourceGadgetIdentifier = reader.Read16BitUnsignedInteger();
        int sourceGadgetStateId = reader.Read16BitUnsignedInteger();
        int sourceGadgetBehaviourNameId = reader.Read16BitUnsignedInteger();
        int targetGadgetIdentifier = reader.Read16BitUnsignedInteger();
        int targetGadgetInputNameId = reader.Read16BitUnsignedInteger();

        return new GadgetLinkData
        {
            SourceGadgetIdentifier = new GadgetIdentifier(sourceGadgetIdentifier),
            SourceGadgetStateId = sourceGadgetStateId,
            SourceLinkBehaviourName = new GadgetBehaviourName(_stringIdLookup[sourceGadgetBehaviourNameId]),
            TargetGadgetIdentifier = new GadgetIdentifier(targetGadgetIdentifier),
            TargetGadgetInputName = new GadgetTriggerName(_stringIdLookup[targetGadgetInputNameId])
        };
    }
}
