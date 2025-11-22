using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Timer;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager :
    IItemManager<LemmingManager>,
    IItemManager<GadgetManager>,
    IItemManager<LevelTimer>,
    IDisposable
{
    private readonly SnapshotRecorder<LemmingManager, Lemming> _lemmingSnapshotRecorder;
    private readonly SnapshotRecorder<GadgetManager, GadgetBase> _gadgetSnapshotRecorder;
    private readonly SnapshotRecorder<SkillSetManager, SkillTrackingData> _skillSetSnapshotRecorder;

    private readonly SnapshotRecorder<RewindManager, LemmingManager> _lemmingManagerSnapshotRecorder;
    private readonly SnapshotRecorder<RewindManager, LevelTimer> _levelTimerRecorder;

    private readonly LevelEventList<SkillAssignmentEventData> _skillAssignmentList;

    private int _maxElapsedTicks;

    public RewindManager(
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        SkillSetManager skillSetManager)
    {
        _lemmingSnapshotRecorder = new SnapshotRecorder<LemmingManager, Lemming>(lemmingManager);
        _gadgetSnapshotRecorder = new SnapshotRecorder<GadgetManager, GadgetBase>(gadgetManager);
        _skillSetSnapshotRecorder = new SnapshotRecorder<SkillSetManager, SkillTrackingData>(skillSetManager);

        _lemmingManagerSnapshotRecorder = new SnapshotRecorder<RewindManager, LemmingManager>(this);
        _levelTimerRecorder = new SnapshotRecorder<RewindManager, LevelTimer>(this);

        var baseNumberOfSkillAssignments = skillSetManager.EstimateBaseNumberOfSkillAssignments(gadgetManager);

        _skillAssignmentList = new LevelEventList<SkillAssignmentEventData>(baseNumberOfSkillAssignments);
    }

    public unsafe bool DoneSkillAssignmentForTick(int tick) => _skillAssignmentList.TryGetDataForTick(tick, out _);

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if (elapsedTicks % RewindConstants.RewindSnapshotInterval != 0)
            return;

        _lemmingSnapshotRecorder.TakeSnapshot();
        _gadgetSnapshotRecorder.TakeSnapshot();
        _skillSetSnapshotRecorder.TakeSnapshot();

        _lemmingManagerSnapshotRecorder.TakeSnapshot();
        _levelTimerRecorder.TakeSnapshot();
    }

    public unsafe void CheckForReplayAction(int elapsedTicks)
    {
        if (_skillAssignmentList.TryGetDataForTick(elapsedTicks, out SkillAssignmentEventData* dataPointer))
        {
            AssignSkillFromReplay(dataPointer);
        }
    }

    private static unsafe void AssignSkillFromReplay(SkillAssignmentEventData* previouslyRecordedSkillAssignment)
    {
        var lemming = LevelScreen.LemmingManager.GetLemming(previouslyRecordedSkillAssignment->LemmingId);

        ValidateLemmingReplayAction(
            lemming,
            previouslyRecordedSkillAssignment);

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(
            previouslyRecordedSkillAssignment->SkillId,
            previouslyRecordedSkillAssignment->TribeId);

        if (skillTrackingData is null)
            throw new InvalidOperationException("Null skill tracking data in replay!");

        LevelScreen.UpdateScheduler.DoSkillAssignment(skillTrackingData, lemming, true);
    }

    private static unsafe void ValidateLemmingReplayAction(Lemming lemming, SkillAssignmentEventData* previouslyRecordedSkillAssignment)
    {
        if (lemming.AnchorPosition == previouslyRecordedSkillAssignment->LemmingPosition &&
            lemming.State.TribeAffiliation.Id == previouslyRecordedSkillAssignment->TribeId &&
            lemming.Orientation == previouslyRecordedSkillAssignment->LemmingOrientation &&
            lemming.FacingDirection == previouslyRecordedSkillAssignment->LemmingFacingDirection)
            return;

        throw new InvalidOperationException("Desync with replay!");
    }

    public unsafe void RecordSkillAssignment(
        int tick,
        Lemming lemming,
        SkillTrackingData skillTrackingData)
    {
        if (tick < _maxElapsedTicks)
        {
            _maxElapsedTicks = tick;
            _skillAssignmentList.RewindBackTo(tick);

            return;
        }

        SkillAssignmentEventData* newSkillAssignementData = _skillAssignmentList.GetNewDataPointer();

        *newSkillAssignementData = new SkillAssignmentEventData(tick, lemming, skillTrackingData.Skill);
    }

    public unsafe int RewindBackTo(int specifiedTick)
    {
        specifiedTick = Math.Max(specifiedTick, 0);

        var correspondingSnapshotNumber = specifiedTick / RewindConstants.RewindSnapshotInterval;

        _lemmingSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _skillSetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);

        _lemmingManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _levelTimerRecorder.ApplySnapshot(correspondingSnapshotNumber);

        LevelScreen.LemmingManager.ResetLemmingPositions();
        LevelScreen.GadgetManager.ResetGadgetPositions();

        var actualElapsedTick = correspondingSnapshotNumber * RewindConstants.RewindSnapshotInterval;

        var targetTick = actualElapsedTick + 1;

        LevelScreen.TerrainPainter.RewindBackTo(targetTick);
        LevelScreen.LevelTimer.SetElapsedTicks(targetTick);

        if (_skillAssignmentList.TryGetDataForTick(targetTick, out SkillAssignmentEventData* previouslyRecordedSkillAssignment))
        {
            AssignSkillFromReplay(previouslyRecordedSkillAssignment);
        }

        return actualElapsedTick;
    }

    public void RewindBackToPreviousSkillAssignment()
    {
        var tick = _skillAssignmentList.LatestTickWithData();
        RewindBackTo(tick);
    }

    ReadOnlySpan<LemmingManager> IItemManager<LemmingManager>.AllItems => new(in LevelScreen.LemmingManagerRef);
    ReadOnlySpan<GadgetManager> IItemManager<GadgetManager>.AllItems => new(in LevelScreen.GadgetManagerRef);
    ReadOnlySpan<LevelTimer> IItemManager<LevelTimer>.AllItems => new(in LevelScreen.LevelTimerRef);

    public void Dispose()
    {
        _lemmingSnapshotRecorder.Dispose();
        _gadgetSnapshotRecorder.Dispose();
        _skillSetSnapshotRecorder.Dispose();

        _lemmingManagerSnapshotRecorder.Dispose();
        _levelTimerRecorder.Dispose();

        _skillAssignmentList.Dispose();
    }
}
