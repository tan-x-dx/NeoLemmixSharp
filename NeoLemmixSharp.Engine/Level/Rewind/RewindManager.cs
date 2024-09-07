using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager
{
    private readonly SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotRecorder = new(LevelScreen.LemmingManager);
    private readonly SnapshotRecorder<GadgetManager, GadgetBase, int> _gadgetSnapshotRecorder = new(LevelScreen.GadgetManager);

    private int _maxElapsedTicks;

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if (elapsedTicks % LevelConstants.RewindSnapshotInterval == 0)
        {
            _lemmingSnapshotRecorder.TakeSnapshot();
            _gadgetSnapshotRecorder.TakeSnapshot();
        }

        LevelScreen.TerrainPainter.RepaintTerrain();
    }
}