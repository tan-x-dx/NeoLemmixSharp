using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager
{
    private readonly SnapshotManager<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotManager;
    private readonly SnapshotManager<GadgetManager, GadgetBase, int> _gadgetSnapshotManager;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager)
    {
        _lemmingSnapshotManager = new SnapshotManager<LemmingManager, Lemming, LemmingSnapshotData>(lemmingManager);
        _gadgetSnapshotManager = new SnapshotManager<GadgetManager, GadgetBase, int>(gadgetManager);
    }
}