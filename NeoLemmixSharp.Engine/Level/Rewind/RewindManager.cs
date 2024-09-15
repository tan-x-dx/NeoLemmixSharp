using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager : IItemManager<LemmingManager>, IItemManager<GadgetManager>
{
    private readonly SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotRecorder;

    private readonly LemmingManager _lemmingManager;
    private readonly GadgetManager _gadgetManager;

    private readonly SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData> _lemmingManagerSnapshotRecorder;
    private readonly SnapshotRecorder<GadgetManager, GadgetBase, int> _gadgetSnapshotRecorder;

    private int _maxElapsedTicks;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager)
    {
        _lemmingManager = lemmingManager;
        _gadgetManager = gadgetManager;

        _lemmingSnapshotRecorder = new SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData>(lemmingManager);
        _lemmingManagerSnapshotRecorder = new SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData>(this);
        _gadgetSnapshotRecorder = new SnapshotRecorder<GadgetManager, GadgetBase, int>(gadgetManager);
    }

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if (elapsedTicks % LevelConstants.RewindSnapshotInterval == 0)
        {
            _lemmingSnapshotRecorder.TakeSnapshot();
            _lemmingManagerSnapshotRecorder.TakeSnapshot();
            _gadgetSnapshotRecorder.TakeSnapshot();
        }

        LevelScreen.TerrainPainter.RepaintTerrain();
    }

    public int RewindBackTo(int specifiedTick)
    {
        specifiedTick = Math.Max(specifiedTick, 0);

        var correspondingSnapshotNumber = specifiedTick / LevelConstants.RewindSnapshotInterval;

        _lemmingSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _lemmingManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);

        var actualElapsedTick = correspondingSnapshotNumber * LevelConstants.RewindSnapshotInterval;

        LevelScreen.TerrainPainter.RewindBackTo(actualElapsedTick);
        LevelScreen.LevelTimer.SetElapsedTicks(actualElapsedTick);

        return actualElapsedTick;
    }

    public void RewindBackToPreviousSkillAssignment()
    {

    }

    int IItemManager<LemmingManager>.NumberOfItems => 1;
    ReadOnlySpan<LemmingManager> IItemManager<LemmingManager>.AllItems => new(in _lemmingManager);

    int IItemManager<GadgetManager>.NumberOfItems => 1;
    ReadOnlySpan<GadgetManager> IItemManager<GadgetManager>.AllItems => new(in _gadgetManager);
}