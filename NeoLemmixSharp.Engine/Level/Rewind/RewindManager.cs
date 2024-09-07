﻿using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager
{
    private readonly SnapshotManager<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotManager = new(LevelScreen.LemmingManager);
    private readonly SnapshotManager<GadgetManager, GadgetBase, int> _gadgetSnapshotManager = new(LevelScreen.GadgetManager);

    public void Tick(int elapsedTicks)
    {
        if (elapsedTicks % LevelConstants.RewindSnapshotInterval == 0)
        {
            _lemmingSnapshotManager.TakeSnapshot();
            _gadgetSnapshotManager.TakeSnapshot();
        }

        LevelScreen.TerrainPainter.RepaintTerrain();
    }
}