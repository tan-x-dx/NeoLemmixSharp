using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager : IItemManager<LemmingManager>, IItemManager<GadgetManager>
{
    private readonly LemmingManager _lemmingManager;
    private readonly GadgetManager _gadgetManager;

    private readonly SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData> _lemmingSnapshotRecorder;
    private readonly SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData> _lemmingManagerSnapshotRecorder;
    private readonly SnapshotRecorder<GadgetManager, GadgetBase, int> _gadgetSnapshotRecorder;
    private readonly SnapshotRecorder<SkillSetManager, SkillTrackingData, SkillSetSnapshotData> _skillSetSnapshotRecorder;
    private readonly TickOrderedList<SkillAssignmentData> _skillCountChanges;

    private int _maxElapsedTicks;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        SkillSetManager skillSetManager)
    {
        _lemmingManager = lemmingManager;
        _gadgetManager = gadgetManager;

        _lemmingSnapshotRecorder = new SnapshotRecorder<LemmingManager, Lemming, LemmingSnapshotData>(lemmingManager);
        _lemmingManagerSnapshotRecorder = new SnapshotRecorder<RewindManager, LemmingManager, LemmingManagerSnapshotData>(this);
        _gadgetSnapshotRecorder = new SnapshotRecorder<GadgetManager, GadgetBase, int>(gadgetManager);
        _skillSetSnapshotRecorder = new SnapshotRecorder<SkillSetManager, SkillTrackingData, SkillSetSnapshotData>(skillSetManager);

        var baseNumberOfSkillAssignments = skillSetManager.CalculateBaseNumberOfSkillAssignments();

        _skillCountChanges = new TickOrderedList<SkillAssignmentData>(baseNumberOfSkillAssignments);
    }

    public bool DoneSkillAssignmentForTick(int tick) => _skillCountChanges.HasDataForTick(tick);

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if (elapsedTicks % LevelConstants.RewindSnapshotInterval != 0)
            return;

        _lemmingSnapshotRecorder.TakeSnapshot();
        _lemmingManagerSnapshotRecorder.TakeSnapshot();
        _gadgetSnapshotRecorder.TakeSnapshot();
        _skillSetSnapshotRecorder.TakeSnapshot();
    }

    public void CheckForReplayAction(int elapsedTicks)
    {
        ref readonly var previouslyRecordedSkillAssignment = ref _skillCountChanges.TryGetDataForTick(elapsedTicks);
        if (!Unsafe.IsNullRef(in previouslyRecordedSkillAssignment))
        {
            AssignSkillFromReplay(in previouslyRecordedSkillAssignment);
        }
    }

    private void AssignSkillFromReplay(in SkillAssignmentData previouslyRecordedSkillAssignment)
    {
        var lemming = _lemmingManager.AllItems[previouslyRecordedSkillAssignment.LemmingId];

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

        var correspondingSnapshotNumber = specifiedTick / LevelConstants.RewindSnapshotInterval;

        _lemmingSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _lemmingManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _skillSetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);

        var actualElapsedTick = correspondingSnapshotNumber * LevelConstants.RewindSnapshotInterval;

        LevelScreen.TerrainPainter.RewindBackTo(actualElapsedTick);
        LevelScreen.LevelTimer.SetElapsedTicks(actualElapsedTick, false);

        ref readonly var previouslyRecordedSkillAssignment = ref _skillCountChanges.TryGetDataForTick(actualElapsedTick);
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

    int IItemManager<LemmingManager>.NumberOfItems => 1;
    ReadOnlySpan<LemmingManager> IItemManager<LemmingManager>.AllItems => new(in _lemmingManager);

    int IItemManager<GadgetManager>.NumberOfItems => 1;
    ReadOnlySpan<GadgetManager> IItemManager<GadgetManager>.AllItems => new(in _gadgetManager);
}