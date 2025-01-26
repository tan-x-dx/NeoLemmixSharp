using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class PrePlacedLemmingDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.PrePlacedLemmingDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        while (numberOfItemsInSection-- > 0)
        {
            int x = reader.Read16BitUnsignedInteger();
            int y = reader.Read16BitUnsignedInteger();
            uint state = reader.Read32BitUnsignedInteger();

            byte orientationByte = reader.Read8BitUnsignedInteger();
            var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientationByte(orientationByte);
            int teamId = reader.Read8BitUnsignedInteger();
            int initialActionId = reader.Read8BitUnsignedInteger();

            levelData.PrePlacedLemmingData.Add(new LemmingData
            {
                X = x - LevelReadWriteHelpers.PositionOffset,
                Y = y - LevelReadWriteHelpers.PositionOffset,
                State = state,

                Orientation = orientation,
                FacingDirection = facingDirection,

                Team = Team.AllItems[teamId],
                InitialLemmingAction = LemmingAction.AllItems[initialActionId]
            });
        }
    }
}