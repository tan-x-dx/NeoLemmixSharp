using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class PrePlacedLemmingDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.PrePlacedLemmingDataSectionIdentifier;

    public PrePlacedLemmingDataComponentReader(
        Version version)
    {
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.PrePlacedLemmingData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            int x = rawFileData.Read16BitUnsignedInteger();
            int y = rawFileData.Read16BitUnsignedInteger();
            uint state = rawFileData.Read32BitUnsignedInteger();

            int orientationByte = rawFileData.Read8BitUnsignedInteger();
            var dht = DihedralTransformation.Decode(orientationByte);
            int teamId = rawFileData.Read8BitUnsignedInteger();
            int initialActionId = rawFileData.Read8BitUnsignedInteger();

            levelData.PrePlacedLemmingData.Add(new LemmingData
            {
                Position = new Point(x - LevelReadWriteHelpers.PositionOffset, y - LevelReadWriteHelpers.PositionOffset),
                State = state,

                Orientation = dht.Orientation,
                FacingDirection = dht.FacingDirection,

                TeamId = teamId,
                InitialLemmingAction = LemmingAction.AllItems[initialActionId]
            });
        }
    }
}