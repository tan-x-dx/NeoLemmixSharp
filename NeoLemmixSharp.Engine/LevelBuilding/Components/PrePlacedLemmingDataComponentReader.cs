using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class PrePlacedLemmingDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.PrePlacedLemmingDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        while (numberOfItemsInSection-- > 0)
        {
            var x = reader.Read16BitUnsignedInteger();
            var y = reader.Read16BitUnsignedInteger();
            var state = reader.Read32BitUnsignedInteger();

            var orientationByte = reader.Read8BitUnsignedInteger();
            var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientations(orientationByte);
            var teamId = reader.Read8BitUnsignedInteger();
            var initialActionId = reader.Read8BitUnsignedInteger();

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