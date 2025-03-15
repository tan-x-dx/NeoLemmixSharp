using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager : IItemManager<LemmingManager>, IItemManager<GadgetManager>, IItemManager<LevelTimer>
{
    private readonly SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotRecorder;
    private readonly SnapshotRecorder<GadgetManager, GadgetBase, int> _gadgetSnapshotRecorder;
    private readonly SnapshotRecorder<SkillSetManager, SkillTrackingData, SkillSetSnapshotData> _skillSetSnapshotRecorder;

    private readonly SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData> _lemmingManagerSnapshotRecorder;
    private readonly SnapshotRecorder<RewindManager, GadgetManager, int> _gadgetManagerSnapshotRecorder;
    private readonly SnapshotRecorder<RewindManager, LevelTimer, LevelTimerSnapshotData> _levelTimerRecorder;

    private readonly TickOrderedList<SkillAssignmentData> _skillCountChanges;

    private int _maxElapsedTicks;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        SkillSetManager skillSetManager)
    {
        _lemmingSnapshotRecorder = new SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData>(lemmingManager);
        _gadgetSnapshotRecorder = new SnapshotRecorder<GadgetManager, GadgetBase, int>(gadgetManager);
        _skillSetSnapshotRecorder = new SnapshotRecorder<SkillSetManager, SkillTrackingData, SkillSetSnapshotData>(skillSetManager);

        _lemmingManagerSnapshotRecorder = new SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData>(this);
        _gadgetManagerSnapshotRecorder = new SnapshotRecorder<RewindManager, GadgetManager, int>(this);
        _levelTimerRecorder = new SnapshotRecorder<RewindManager, LevelTimer, LevelTimerSnapshotData>(this);

        var baseNumberOfSkillAssignments = skillSetManager.CalculateBaseNumberOfSkillAssignments();

        _skillCountChanges = new TickOrderedList<SkillAssignmentData>(baseNumberOfSkillAssignments);
    }

    public bool DoneSkillAssignmentForTick(int tick) => _skillCountChanges.HasDataForTick(tick);

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if (elapsedTicks % EngineConstants.RewindSnapshotInterval != 0)
            return;

        _lemmingSnapshotRecorder.TakeSnapshot();
        _gadgetSnapshotRecorder.TakeSnapshot();
        _skillSetSnapshotRecorder.TakeSnapshot();

        _lemmingManagerSnapshotRecorder.TakeSnapshot();
        _gadgetManagerSnapshotRecorder.TakeSnapshot();
        _levelTimerRecorder.TakeSnapshot();
    }

    public void CheckForReplayAction(int elapsedTicks)
    {
        ref readonly var previouslyRecordedSkillAssignment = ref _skillCountChanges.TryGetDataForTick(elapsedTicks);
        if (!Unsafe.IsNullRef(in previouslyRecordedSkillAssignment))
        {
            AssignSkillFromReplay(in previouslyRecordedSkillAssignment);
        }
    }

    private static void AssignSkillFromReplay(in SkillAssignmentData previouslyRecordedSkillAssignment)
    {
        var lemming = LevelScreen.LemmingManager.AllLemmings[previouslyRecordedSkillAssignment.LemmingId];

        ValidateLemmingReplayAction(
            lemming,
            in previouslyRecordedSkillAssignment);

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(
            previouslyRecordedSkillAssignment.SkillId,
            previouslyRecordedSkillAssignment.TeamId);

        if (skillTrackingData is null)
            throw new InvalidOperationException("Null skill tracking data in replay!");

        LevelScreen.UpdateScheduler.DoSkillAssignment(skillTrackingData, lemming, true);
    }

    private static void ValidateLemmingReplayAction(Lemming lemming, in SkillAssignmentData previouslyRecordedSkillAssignment)
    {
        if (lemming.LevelPosition == previouslyRecordedSkillAssignment.LemmingPosition &&
            lemming.State.TeamAffiliation.Id == previouslyRecordedSkillAssignment.TeamId &&
            lemming.Orientation.RotNum == previouslyRecordedSkillAssignment.LemmingOrientationRotNum &&
            lemming.FacingDirection.Id == previouslyRecordedSkillAssignment.LemmingFacingDirectionId)
            return;

        throw new InvalidOperationException("Desync with replay!");
    }

    public void RecordSkillAssignment(
        int tick,
        Lemming lemming,
        SkillTrackingData skillTrackingData)
    {
        if (tick < _maxElapsedTicks)
        {
            _maxElapsedTicks = tick;
            _skillCountChanges.RewindBackTo(tick);

            return;
        }

        ref var skillAssignmentData = ref _skillCountChanges.GetNewDataRef();

        skillAssignmentData = new SkillAssignmentData(tick, lemming, skillTrackingData.Skill);
    }

    public int RewindBackTo(int specifiedTick)
    {
        specifiedTick = Math.Max(specifiedTick, 0);

        var correspondingSnapshotNumber = specifiedTick / EngineConstants.RewindSnapshotInterval;

        _lemmingSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _skillSetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);

        _lemmingManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _levelTimerRecorder.ApplySnapshot(correspondingSnapshotNumber);

        var actualElapsedTick = correspondingSnapshotNumber * EngineConstants.RewindSnapshotInterval;

        LevelScreen.TerrainPainter.RewindBackTo(actualElapsedTick + 1);
        LevelScreen.LevelTimer.SetElapsedTicks(actualElapsedTick + 1, false);

        ref readonly var previouslyRecordedSkillAssignment = ref _skillCountChanges.TryGetDataForTick(actualElapsedTick + 1);
        if (!Unsafe.IsNullRef(in previouslyRecordedSkillAssignment))
        {
            AssignSkillFromReplay(in previouslyRecordedSkillAssignment);
        }

        return actualElapsedTick;
    }

    public void RewindBackToPreviousSkillAssignment()
    {
        var tick = _skillCountChanges.LatestTickWithData();
        RewindBackTo(tick);
    }

    public int NumberOfItems => 1;

    ReadOnlySpan<LemmingManager> IItemManager<LemmingManager>.AllItems => new(in LevelScreen.LemmingManagerRef);
    ReadOnlySpan<GadgetManager> IItemManager<GadgetManager>.AllItems => new(in LevelScreen.GadgetManagerRef);
    ReadOnlySpan<LevelTimer> IItemManager<LevelTimer>.AllItems => new(in LevelScreen.LevelTimerRef);
}