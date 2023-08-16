using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Skills;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelCursor
{
#pragma warning disable CS8618
    public static LevelScreen LevelScreen { private get; set; }
#pragma warning restore CS8618

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelInputController _controller;
    private readonly SkillSetManager _skillSetManager;

    private LemmingSkill _skill = NoneSkill.Instance;
    private Lemming? _lemmingUnderCursor;
    private Lemming? _queuedLemming;
    private FacingDirection _facingDirection = LeftFacingDirection.Instance;
    private int _numberOfLemmingsUnderCursor;
    private int _curValue;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;
    private bool _selectOnlyFacingLeft;
    private bool _selectOnlyFacingRight;

    private bool _hitTestAutoFail;
    private bool _doneAssignmentThisFrame;

    public bool HighlightLemming { get; private set; }
    public bool LemmingsUnderCursor => _numberOfLemmingsUnderCursor > 0;

    public bool CursorOnLevel { get; set; }
    public LevelPosition CursorPosition { get; set; }

    public LevelCursor(
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour,
        ILevelControlPanel controlPanel,
        LevelInputController controller,
        SkillSetManager skillSetManager)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _controlPanel = controlPanel;
        _controller = controller;
        _skillSetManager = skillSetManager;
    }

    public void OnNewFrame()
    {
        _lemmingUnderCursor = null;
        _queuedLemming = null;
        _skill = _controlPanel.SelectedSkill;
        _numberOfLemmingsUnderCursor = 0;
        _curValue = 10;

        _selectOnlyWalkers = _controller.SelectOnlyWalkers.IsKeyDown;
        _selectOnlyUnassigned = _controller.SelectOnlyUnassignedLemmings.IsKeyDown;

        if (_controller.SelectLeftFacingLemmings.IsKeyDown)
        {
            _selectOnlyFacingLeft = true;
            _facingDirection = LeftFacingDirection.Instance;
        }

        if (_controller.SelectRightFacingLemmings.IsKeyDown)
        {
            _selectOnlyFacingRight = true;
            _facingDirection = RightFacingDirection.Instance;
        }
    }

    public void CheckLemming(Lemming lemming)
    {
        if (!CursorOnLevel)
            return;

        if (HighlightLemming && !false /*(L = GetHighlitLemming)*/)
            return;

        /* if (lemming.IsRemoved || lemming.IsTeleporting)
         return;*/
        /*

        // Check if we only look for highlighted Lems
        if (IsHighlight and not(L = GetHighlitLemming))
        or(IsReplay and not(L = GetTargetLemming)) 
            return;
        // Does Lemming exist
        if L.LemRemoved or L.LemTeleporting 
            return;
        // Is the Lemming unable to receive skills, because zombie, neutral, or was-ohnoer? (remove unless we haven't yet had any lem under the cursor)
        if L.CannotReceiveSkills and Assigned(PriorityLem) 
            return;
        */

        // Is Lemming inside cursor (only check if we are not using Highlighting!)
        if (!LemmingIsUnderCursor(lemming)) // and(not(IsHighlight or IsReplay))
            return;

        // Directional select
        if ((_selectOnlyFacingLeft != _selectOnlyFacingRight) &&
            lemming.FacingDirection != _facingDirection &&
            !(false))//and(not(IsHighlight or IsReplay))
            return;

        // Select only walkers
        if (_selectOnlyWalkers && lemming.CurrentAction != WalkerAction.Instance &&
            !(false)) //and(not(IsHighlight or IsReplay))
            return;

        // Increase number of lemmings in cursor (if not a zombie or neutral)
        //if(lemming.CanReceiveSkills)
        //{
        _numberOfLemmingsUnderCursor++;
        //}

        // Determine priority class of current lemming
        int currentPriorityValue;
        if (_selectOnlyUnassigned || _selectOnlyWalkers)
        {
            currentPriorityValue = 1;
        }
        else
        {
            currentPriorityValue = 0;
            bool lemIsInBox;
            do
            {
                lemIsInBox = LemmingIsPriority(lemming, currentPriorityValue);
                currentPriorityValue++;
            } while (currentPriorityValue > Math.Min(_curValue, 4) || lemIsInBox);
        }

        // Can this lemming actually receive the skill?
        if (!_skill.CanAssignToLemming(lemming))
        {
            currentPriorityValue = 8;
        }

        // Deprioritize zombie even when just counting lemmings
        if (false) //(!lemming.CanReceiveSkills)
        {
            currentPriorityValue = 9;
        }

        if (currentPriorityValue < _curValue ||
            (currentPriorityValue == _curValue && IsCloserToCursorCentre(_lemmingUnderCursor, lemming)))
        {
            // New top priority lemming found
            _lemmingUnderCursor = lemming;
            _queuedLemming = lemming;
            _curValue = currentPriorityValue;
        }

        //  Delete PriorityLem if too low-priority and we wish to assign a skill
        if (_curValue > 6 && _skill != NoneSkill.Instance)
        {
            _queuedLemming = _lemmingUnderCursor;
            _lemmingUnderCursor = null;
        }
    }

    // TODO Make this cleverer
    private static bool LemmingIsPriority(Lemming lemming, int priorityValue)
    {
        var action = lemming.CurrentAction;

        return priorityValue switch
        {
            0 => action == BasherAction.Instance ||
                 action == FencerAction.Instance ||
                 action == MinerAction.Instance ||
                 action == DiggerAction.Instance ||
                 action == BuilderAction.Instance ||
                 action == PlatformerAction.Instance ||
                 action == StackerAction.Instance ||
                 action == BlockerAction.Instance ||
                 action == ShruggerAction.Instance ||
                 action == ReacherAction.Instance ||
                 action == ShimmierAction.Instance ||
                 action == LasererAction.Instance,

            1 => lemming.HasPermanentSkill,

            2 => !(action == WalkerAction.Instance ||
                   action == AscenderAction.Instance),

            3 => action == WalkerAction.Instance ||
                 action == AscenderAction.Instance,

            _ => true
        };
    }

    public void HandleMouseInput()
    {
        // CheckLemmingsUnderCursor();

        if (_controller.LeftMouseButtonAction.IsPressed)
        {
            HandleSkillAssignment();
        }
    }

    private bool HandleSkillAssignment(bool isReplayAssignment = false)
    {
        if (_skill == NoneSkill.Instance)
            return false;

        if (_lemmingUnderCursor == null || !SkillIsAvailable(_skill))
        {
            if (_queuedLemming != null && !_skill.IsPermanentSkill)
            {
                LevelScreen.SetQueuedSkill(_queuedLemming, _skill);
            }

            return false;
        }

        if (isReplayAssignment) // If the assignment is written in the replay, change lemming state
        {
            if (!AssignSkill())
                return false;

            // CueSoundEffect(SFX_ASSIGN_SKILL, L.Position);

            return true;
        }

        if (!SkillIsAvailable(_skill))
            return false;

        RegainControl();
        RecordSkillAssignment(_lemmingUnderCursor, _skill);
        AssignSkill();

        return true;
    }

    private bool LemmingIsUnderCursor(Lemming lemming)
    {
        if (!CursorOnLevel)
            return false;

        var lemmingPosition = lemming.Orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX, 4);

        var dx = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.Y, lemmingPosition.Y);

        return dx < 4 && dy < 4;
    }

    private bool IsCloserToCursorCentre(Lemming? previousCandidate, Lemming newCandidate)
    {
        if (previousCandidate is null)
            return true;

        var previousLemmingPosition = previousCandidate.Orientation.Move(previousCandidate.LevelPosition, previousCandidate.FacingDirection.DeltaX, 4);
        var newLemmingPosition = newCandidate.Orientation.Move(newCandidate.LevelPosition, newCandidate.FacingDirection.DeltaX, 4);

        var dx1 = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.X, previousLemmingPosition.X);
        var dy1 = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.Y, previousLemmingPosition.Y);

        var dx2 = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.X, newLemmingPosition.X);
        var dy2 = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.Y, newLemmingPosition.Y);

        return dx2 * dx2 + dy2 * dy2 < dx1 * dx1 + dy1 * dy1;
    }

    private bool SkillIsAvailable(LemmingSkill lemmingSkill)
    {
        return _skillSetManager.SkillIsAvailable(lemmingSkill);
    }

    private bool AssignSkill()
    {
        if (!SkillIsAvailable(_skill) || _doneAssignmentThisFrame)
            return false;

        //UpdateSkillCount(lemmingSkill);

        LevelScreen.ClearQueuedSkill();

        var result = _skill.AssignToLemming(_lemmingUnderCursor!);

        _doneAssignmentThisFrame = true;

        return result;
    }

    private void RegainControl()
    {
    }

    private void RecordSkillAssignment(Lemming queuedLemming, LemmingSkill skill)
    {
    }
}