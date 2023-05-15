using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class LevelUpdater
{
    private readonly IFrameUpdater _standardFrameUpdater;
    private readonly IFrameUpdater _fastForwardFrameUpdater;

    private IFrameUpdater _currentlySelectedFrameUpdater;

    public bool IsFastForwards { get; private set; }

    public bool DoneAssignmentThisFrame { get; set; }
    public LemmingSkill QueuedSkill { get; private set; } = NoneSkill.Instance;
    public Lemming? QueuedSkillLemming { get; private set; }
    public int QueuedSkillFrame { get; private set; }

    public LevelUpdater(bool isSuperLemmingMode)
    {
        _fastForwardFrameUpdater = new FastForwardsFrameUpdater();
        _standardFrameUpdater = isSuperLemmingMode
            ? _fastForwardFrameUpdater
            : new StandardFrameUpdater();

        _currentlySelectedFrameUpdater = _standardFrameUpdater;
    }

    public void ToggleFastForwards()
    {
        if (IsFastForwards)
        {
            _currentlySelectedFrameUpdater = _standardFrameUpdater;
            IsFastForwards = false;
        }
        else
        {
            _currentlySelectedFrameUpdater = _fastForwardFrameUpdater;
            IsFastForwards = true;
        }
    }

    public void UpdateLemming(Lemming lemming)
    {
        _currentlySelectedFrameUpdater.UpdateLemming(lemming);
    }

    public void Update()
    {
        _currentlySelectedFrameUpdater.Update();
    }

    public void ClearQueuedSkill()
    {
        QueuedSkill = NoneSkill.Instance;
        QueuedSkillLemming = null;
        QueuedSkillFrame = 0;
    }

    public void SetQueuedSkill(Lemming lemming, LemmingSkill lemmingSkill)
    {
        QueuedSkill = lemmingSkill;
        QueuedSkillLemming = lemming;
        QueuedSkillFrame = 0;
    }

    public void CheckForQueuedAction()
    {
        // First check whether there was already a skill assignment this frame
        //    if Assigned(fReplayManager.Assignment[fCurrentIteration, 0]) then Exit;

        if (QueuedSkill == NoneSkill.Instance || QueuedSkillLemming == null)
        {
            ClearQueuedSkill();
            return;
        }

        if (false) //(lemming.IsRemoved || !lemming.CanReceiveSkills || lemming.IsTeleporting) // CannotReceiveSkills covers neutral and zombie
        {
            // delete queued action first
            ClearQueuedSkill();
            return;
        }

        if (QueuedSkill.CanAssignToLemming(QueuedSkillLemming) && QueuedSkill.CurrentNumberOfSkillsAvailable > 0)
        {
            // Record skill assignment, so that we apply it in CheckForReplayAction
            // RecordSkillAssignment(L, NewSkill)
        }
        else
        {
            QueuedSkillFrame++;

            // Delete queued action after 16 frames
            if (QueuedSkillFrame > 15)
            {
                ClearQueuedSkill();
            }
        }
    }
}