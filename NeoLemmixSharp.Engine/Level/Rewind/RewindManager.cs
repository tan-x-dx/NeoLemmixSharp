using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Objectives;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class RewindManager : IDisposable
{
    private readonly SnapshotRecorder _lemmingSnapshotRecorder;
    private readonly SnapshotRecorder _lemmingManagerSnapshotRecorder;
    private readonly SnapshotRecorder _gadgetSnapshotRecorder;
    private readonly SnapshotRecorder _hatchGroupSnapshotRecorder;
    private readonly SnapshotRecorder _skillSetSnapshotRecorder;
    private readonly SnapshotRecorder _levelTimerRecorder;

    private readonly LevelEventList<SkillAssignmentEventData> _skillAssignmentList;

    private int _maxElapsedTicks;

    public RewindManager(
        RawArray lemmingDataBuffer,
        RawArray lemmingManagerDataBuffer,
        RawArray gadgetDataBuffer,
        RawArray hatchGroupDataBuffer,
        RawArray levelTimerDataBuffer,
        RawArray skillSetDataBuffer,
        int baseNumberOfSkillAssignments)
    {
        _lemmingSnapshotRecorder = new SnapshotRecorder(lemmingDataBuffer);
        _lemmingManagerSnapshotRecorder = new SnapshotRecorder(lemmingManagerDataBuffer);
        _gadgetSnapshotRecorder = new SnapshotRecorder(gadgetDataBuffer);
        _hatchGroupSnapshotRecorder = new SnapshotRecorder(hatchGroupDataBuffer);
        _skillSetSnapshotRecorder = new SnapshotRecorder(skillSetDataBuffer);
        _levelTimerRecorder = new SnapshotRecorder(levelTimerDataBuffer);

        _skillAssignmentList = new LevelEventList<SkillAssignmentEventData>(baseNumberOfSkillAssignments);
    }

    public unsafe bool DoneSkillAssignmentForTick(int tick) => _skillAssignmentList.TryGetDataForTick(tick, out _);

    public void Tick(int elapsedTicks)
    {
        _maxElapsedTicks = Math.Max(_maxElapsedTicks, elapsedTicks);

        if ((elapsedTicks % RewindConstants.RewindSnapshotInterval) != 0)
            return;

        _lemmingSnapshotRecorder.TakeSnapshot();
        _lemmingManagerSnapshotRecorder.TakeSnapshot();
        _gadgetSnapshotRecorder.TakeSnapshot();
        _hatchGroupSnapshotRecorder.TakeSnapshot();
        _skillSetSnapshotRecorder.TakeSnapshot();
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

        var skillTrackingData = LevelScreen.SkillSetManager.TryGetSkillTrackingData(
            previouslyRecordedSkillAssignment->SkillId,
            previouslyRecordedSkillAssignment->TribeId);

        if (skillTrackingData is null)
            throw new InvalidOperationException("Null skill tracking data in replay!");

        LevelScreen.UpdateScheduler.DoSkillAssignment(skillTrackingData, lemming, true);
    }

    private static unsafe void ValidateLemmingReplayAction(Lemming lemming, SkillAssignmentEventData* previouslyRecordedSkillAssignment)
    {
        if (lemming.AnchorPosition == previouslyRecordedSkillAssignment->LemmingPosition &&
            lemming.State.TribeId == previouslyRecordedSkillAssignment->TribeId &&
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

        *newSkillAssignementData = new SkillAssignmentEventData(tick, lemming, skillTrackingData.LemmingSkillId);
    }

    public unsafe int RewindBackTo(int specifiedTick)
    {
        specifiedTick = Math.Max(specifiedTick, 0);

        var correspondingSnapshotNumber = specifiedTick / RewindConstants.RewindSnapshotInterval;

        _lemmingSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _lemmingManagerSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _gadgetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _hatchGroupSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _skillSetSnapshotRecorder.ApplySnapshot(correspondingSnapshotNumber);
        _levelTimerRecorder.ApplySnapshot(correspondingSnapshotNumber);

        LevelScreen.LemmingManager.OnSnapshotApplied();
        LevelScreen.GadgetManager.OnSnapshotApplied();
        LevelScreen.SkillSetManager.OnSnapshotApplied();

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

    public void Dispose()
    {
        _lemmingSnapshotRecorder.Dispose();
        _lemmingManagerSnapshotRecorder.Dispose();
        _gadgetSnapshotRecorder.Dispose();
        _hatchGroupSnapshotRecorder.Dispose();
        _skillSetSnapshotRecorder.Dispose();
        _levelTimerRecorder.Dispose();

        _skillAssignmentList.Dispose();

        GC.SuppressFinalize(this);
    }
}
