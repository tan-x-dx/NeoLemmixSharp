using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager
{
    private readonly SnapshotManager<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotManager;
    private readonly SnapshotManager<GadgetManager, GadgetBase, int> _gadgetSnapshotManager;

    private readonly TerrainPainter _terrainPainter;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        TerrainPainter terrainPainter)
    {
        _lemmingSnapshotManager = new SnapshotManager<LemmingManager, Lemming, LemmingSnapshotData>(lemmingManager);
        _gadgetSnapshotManager = new SnapshotManager<GadgetManager, GadgetBase, int>(gadgetManager);
        _terrainPainter = terrainPainter;
    }

    public void Tick(int elapsedTicks)
    {
        if (elapsedTicks % LevelConstants.RewindSnapshotInterval == 0)
        {
            _lemmingSnapshotManager.TakeSnapshot();
            _gadgetSnapshotManager.TakeSnapshot();
        }

        _terrainPainter.RepaintTerrain();
    }
}