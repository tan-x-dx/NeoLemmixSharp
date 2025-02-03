using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;
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

        while (numberOfItemsInSection-- > 0)
        {
            int x = rawFileData.Read16BitUnsignedInteger();
            int y = rawFileData.Read16BitUnsignedInteger();
            uint state = rawFileData.Read32BitUnsignedInteger();

            byte orientationByte = rawFileData.Read8BitUnsignedInteger();
            LevelReadWriteHelpers.DecipherOrientationByte(orientationByte, out var orientation, out var facingDirection);
            int teamId = rawFileData.Read8BitUnsignedInteger();
            int initialActionId = rawFileData.Read8BitUnsignedInteger();

            levelData.PrePlacedLemmingData.Add(new LemmingData
            {
                X = x - LevelReadWriteHelpers.PositionOffset,
                Y = y - LevelReadWriteHelpers.PositionOffset,
                State = state,

                Orientation = orientation,
                FacingDirection = facingDirection,

                TeamId = teamId,
                InitialLemmingAction = LemmingAction.AllItems[initialActionId]
            });
        }
    }
}