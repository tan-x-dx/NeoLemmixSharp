using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class TerrainDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public TerrainDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        //levelData.TerrainArchetypeData.Add(new TerrainArchetypeData
        //{
            
        //});
        

        while (numberOfItemsInSection-- > 0)
        {
            var bytesToRead = reader.Read8BitUnsignedInteger();

            var style = _stringIdLookup[reader.Read16BitUnsignedInteger()];
            var piece = _stringIdLookup[reader.Read16BitUnsignedInteger()];

            var x = reader.Read16BitUnsignedInteger();
            var y = reader.Read16BitUnsignedInteger();





            var state = reader.Read32BitUnsignedInteger();

            var orientationByte = reader.Read8BitUnsignedInteger();
            var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientations(orientationByte);
            var teamId = reader.Read8BitUnsignedInteger();
            var initialActionId = reader.Read8BitUnsignedInteger();

            levelData.AllTerrainData.Add(new TerrainData
            {


                X = x - LevelReadWriteHelpers.PositionOffset,
                Y = y - LevelReadWriteHelpers.PositionOffset,
                //State = state,

                //Orientation = orientation,
                //FacingDirection = facingDirection,

                //Team = Team.AllItems[teamId],
                //InitialLemmingAction = LemmingAction.AllItems[initialActionId]
            });
        }
    }
}