﻿using NeoLemmixSharp.Common;
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

        while (numberOfItemsInSection-- > 0)
        {
            int x = rawFileData.Read16BitUnsignedInteger();
            int y = rawFileData.Read16BitUnsignedInteger();
            uint state = rawFileData.Read32BitUnsignedInteger();

            uint orientationByte = rawFileData.Read8BitUnsignedInteger();
            var dht = DihedralTransformation.DecodeFromUint(orientationByte);
            int teamId = rawFileData.Read8BitUnsignedInteger();
            int initialActionId = rawFileData.Read8BitUnsignedInteger();

            levelData.PrePlacedLemmingData.Add(new LemmingData
            {
                X = x - LevelReadWriteHelpers.PositionOffset,
                Y = y - LevelReadWriteHelpers.PositionOffset,
                State = state,

                Orientation = dht.Orientation,
                FacingDirection = dht.FacingDirection,

                TeamId = teamId,
                InitialLemmingAction = LemmingAction.AllItems[initialActionId]
            });
        }
    }
}