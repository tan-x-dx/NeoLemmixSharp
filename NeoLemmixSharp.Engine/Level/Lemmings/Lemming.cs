using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IEquatable<Lemming>, IRectangularBounds
{
    private readonly LemmingData _data;

    public LemmingColors LemmingColors;

    public LemmingRenderer Renderer { get; }

    public int Id { get; }

    public LemmingActionType PreviousActionType => _data.PreviousActionType;
    public LemmingAction PreviousAction => LemmingAction.GetActionOrDefault(_data.PreviousActionType);

    public LemmingActionType CurrentActionType => _data.CurrentActionType;
    public LemmingAction CurrentAction
    {
        get => LemmingAction.GetActionOrDefault(_data.CurrentActionType);
        set
        {
            _data.CurrentActionType = value.ActionType;
            Renderer.UpdateLemmingState(true);
        }
    }

    public LemmingActionType NextActionType => _data.NextActionType;
    public LemmingAction NextAction
    {
        get => LemmingAction.GetActionOrDefault(_data.NextActionType);
        set => _data.NextActionType = value.ActionType;
    }

    public LemmingAction CountDownAction => LemmingAction.GetActionOrDefault(_data.CountDownActionType);

    public Orientation Orientation
    {
        get => _data.Orientation;
        set
        {
            _data.Orientation = value;
            Renderer.UpdateLemmingState(true);
        }
    }

    public FacingDirection FacingDirection
    {
        get => _data.FacingDirection;
        set
        {
            _data.FacingDirection = value;
            Renderer.UpdateLemmingState(true);
        }
    }

    public DihedralTransformation GetDihedralTransformation() => _data.GetDihedralTransformation();

    RectangularRegion IRectangularBounds.CurrentBounds => _data.CurrentBounds;
    public ref RectangularRegion CurrentBounds => ref _data.CurrentBounds;

    public ref Point DehoistPin => ref _data.DehoistPin;
    public ref Point LaserHitLevelPosition => ref _data.LaserHitLevelPosition;
    public ref Point AnchorPosition => ref _data.AnchorPosition;
    public ref Point PreviousAnchorPosition => ref _data.PreviousAnchorPosition;

    public ref bool ConstructivePositionFreeze => ref _data.ConstructivePositionFreeze;
    public ref bool IsStartingAction => ref _data.IsStartingAction;
    public ref bool PlacedBrick => ref _data.PlacedBrick;
    public ref bool StackLow => ref _data.StackLow;
    public ref bool InitialFall => ref _data.InitialFall;
    public ref bool EndOfAnimation => ref _data.EndOfAnimation;
    public ref bool LaserHit => ref _data.LaserHit;
    public ref bool JumpToHoistAdvance => ref _data.JumpToHoistAdvance;
    public ref int AnimationFrame => ref _data.AnimationFrame;
    public ref int PhysicsFrame => ref _data.PhysicsFrame;
    public ref int AscenderProgress => ref _data.AscenderProgress;
    public ref int NumberOfBricksLeft => ref _data.NumberOfBricksLeft;
    public ref int DisarmingFrames => ref _data.DisarmingFrames;
    public ref int DistanceFallen => ref _data.DistanceFallen;
    public ref int JumpProgress => ref _data.JumpProgress;
    public ref int TrueDistanceFallen => ref _data.TrueDistanceFallen;
    public ref int LaserRemainTime => ref _data.LaserRemainTime;
    public ref uint CountDownTimer => ref _data.CountDownTimer;
    public ref int ParticleTimer => ref _data.ParticleTimer;

    public bool IsSimulation => Id == EngineConstants.SimulationLemmingId;
    public bool IsFastForward => _data.FastForwardTime > 0 || IsPermanentFastForwards;

    public Point HeadPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 6);
    public Point FootPosition => CurrentAction.GetFootPosition(this, _data.AnchorPosition);
    public Point CenterPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 4);

    public Span<Point> GetJumperPositions() => _data.GetJumperPositions();

    #region Lemming State Properties

    public bool HasPermanentSkill => (_data.State & LemmingAbilityConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_data.State & LemmingAbilityConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_data.State & LemmingAbilityConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_data.State & LemmingAbilityConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_data.State & LemmingAbilityConstants.AssignableSkillBitMask) == (1U << LemmingAbilityConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_data.State >>> LemmingAbilityConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ClimberBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_data.State >>> LemmingAbilityConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.FloaterBitIndex;
                states &= ~(1U << LemmingAbilityConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_data.State >>> LemmingAbilityConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.GliderBitIndex;
                states &= ~(1U << LemmingAbilityConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_data.State >>> LemmingAbilityConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.SliderBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_data.State >>> LemmingAbilityConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.SwimmerBitIndex;
                states &= ~((1U << LemmingAbilityConstants.AcidLemmingBitIndex) | (1U << LemmingAbilityConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_data.State >>> LemmingAbilityConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.DisarmerBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_data.State >>> LemmingAbilityConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.PermanentFastForwardBitIndex);
            }
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
        }
    }

    public bool IsAcidLemming
    {
        get => ((_data.State >>> LemmingAbilityConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.AcidLemmingBitIndex;
                states &= ~((1U << LemmingAbilityConstants.SwimmerBitIndex) | (1U << LemmingAbilityConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_data.State >>> LemmingAbilityConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.WaterLemmingBitIndex;
                states &= ~((1U << LemmingAbilityConstants.SwimmerBitIndex) | (1U << LemmingAbilityConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_data.State >>> LemmingAbilityConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ActiveBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsNeutral
    {
        get => ((_data.State >>> LemmingAbilityConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.NeutralBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => ((_data.State >>> LemmingAbilityConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            ref var states = ref _data.State;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ZombieBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ZombieBitIndex);
            }
            LevelScreen.LemmingManager.UpdateZombieState(this);
            UpdateSkinColor();
        }
    }

    public int TribeId => _data.TribeId;

    #endregion

    public Lemming(
        ref nint dataHandle,
        int id)
    {
        Id = id;
        _data = PointerDataHelper.CreateItem<LemmingData>(ref dataHandle);

        Renderer = new LemmingRenderer(this);
    }

    public void Initialise()
    {
        IsActive = true;
        _data.PreviousAnchorPosition = _data.AnchorPosition;

        var initialAction = CurrentAction;
        _data.CurrentBounds = initialAction.GetLemmingBounds(this);

        if (initialAction.ActionType == LemmingActionType.NoneAction)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);

        UpdateAllColors();

        Renderer.UpdateLemmingState(true);
    }

    [SkipLocalsInit]
    public unsafe void Tick()
    {
        _data.PreviousActionType = _data.CurrentActionType;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        Point* gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];
        Point* p = gadgetCheckPositions;

        // Use first four entries of span to hold level positions.
        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the minimum bounding box of four level positions:
        // the anchor and foot positions of the previous frame, and a large box around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var positionTemp = _data.Orientation.Move(_data.AnchorPosition, -5, -12);
        *p = positionTemp;
        p++;
        positionTemp = _data.Orientation.Move(_data.AnchorPosition, 5, 12);
        *p = positionTemp;
        p++;
        positionTemp = _data.PreviousAnchorPosition;
        *p = positionTemp;
        p++;
        positionTemp = PreviousAction.GetFootPosition(this, positionTemp);
        *p = positionTemp;

        var checkPositionsBounds = new RectangularRegion(Helpers.CreateReadOnlySpan<Point>(gadgetCheckPositions, 4));

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        EvaluateLemmingLogic(in gadgetsNearLemming, Helpers.CreateSpan<Point>(gadgetCheckPositions, LemmingMovementHelper.MaxIntermediateCheckPositions));
    }

    private void EvaluateLemmingLogic(
        in GadgetEnumerable gadgetsNearLemming,
        Span<Point> gadgetCheckPositions)
    {
        if (!HandleLemmingAction(in gadgetsNearLemming)) return;
        if (!CheckLevelBoundaries()) return;
        if (!CheckTriggerAreas(in gadgetsNearLemming, gadgetCheckPositions, false)) return;
        if (CurrentActionType == LemmingActionType.ExiterAction) return;
        if (IsZombie) return;
        if (!LevelScreen.LemmingManager.AnyZombies()) return;

        LevelScreen.LemmingManager.DoZombieCheck(this);
    }

    [SkipLocalsInit]
    public unsafe void Simulate(bool checkGadgets)
    {
        if (!IsSimulation)
            throw new InvalidOperationException("Use simulation lemming for simulations!");

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        Point* gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];
        Point* p = gadgetCheckPositions;

        // Use first four entries of span to hold level positions.
        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the minimum bounding box of four level positions:
        // the anchor and foot positions of the previous frame, and a large box around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var positionTemp = _data.Orientation.Move(_data.AnchorPosition, -5, -12);
        *p = positionTemp;
        p++;
        positionTemp = _data.Orientation.Move(_data.AnchorPosition, 5, 12);
        *p = positionTemp;
        p++;
        positionTemp = _data.PreviousAnchorPosition;
        *p = positionTemp;
        p++;
        positionTemp = PreviousAction.GetFootPosition(this, positionTemp);
        *p = positionTemp;

        var checkPositionsBounds = new RectangularRegion(Helpers.CreateReadOnlySpan<Point>(gadgetCheckPositions, 4));

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        var handleGadgets = HandleLemmingAction(in gadgetsNearLemming) && CheckLevelBoundaries() && checkGadgets;
        if (handleGadgets)
        {
            // Reuse the above span. LemmingMovementHelper will overwrite existing values
            CheckTriggerAreas(in gadgetsNearLemming, Helpers.CreateSpan<Point>(gadgetCheckPositions, LemmingMovementHelper.MaxIntermediateCheckPositions), false);
        }
    }

    private void HandleParticleTimer()
    {
        if (_data.ParticleTimer > 0)
        {
            _data.ParticleTimer--;
        }
    }

    private void HandleCountDownTimer()
    {
        if (_data.CountDownTimer == 0)
            return;

        _data.CountDownTimer--;
        CountDownHelper.UpdateCountDownTimer(this);

        if (_data.CountDownTimer != 0)
            return;

        OhNoerAction.HandleCountDownTransition(this);
    }

    private void HandleFastForwardTimer()
    {
        ref var fastForwardTime = ref _data.FastForwardTime;

        if (fastForwardTime <= 0)
            return;

        fastForwardTime--;

        if (fastForwardTime == 0)
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    /// <summary>
    /// Handle one frame of the lemming's action, and update data accordingly.
    /// Some actions (e.g. exiter, splatter, etc) may result in the lemming being killed off in some way.
    /// In that case, the method returns false - no extra work is necessary (don't bother checking gadgets, for example).
    /// </summary>
    /// <returns>True if more work needs to be done this frame</returns>
    private bool HandleLemmingAction(in GadgetEnumerable gadgetsNearLemming)
    {
        var currentAction = CurrentAction;

        var frame = _data.AnimationFrame + 1;
        if (frame == currentAction.NumberOfAnimationFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentAction.ActionType == LemmingActionType.FloaterAction ||
                currentAction.ActionType == LemmingActionType.GliderAction)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }
        }
        _data.AnimationFrame = frame;

        frame = _data.PhysicsFrame + 1;
        if (frame == currentAction.MaxPhysicsFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentAction.ActionType == LemmingActionType.FloaterAction ||
                currentAction.ActionType == LemmingActionType.GliderAction)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }

            _data.EndOfAnimation = currentAction.IsOneTimeAction();
        }

        _data.PhysicsFrame = frame;
        _data.PreviousAnchorPosition = _data.AnchorPosition;

        var result = currentAction.UpdateLemming(this, in gadgetsNearLemming);
        _data.CurrentBounds = currentAction.GetLemmingBounds(this);

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        var terrainManager = LevelScreen.TerrainManager;
        var footPixel = terrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = terrainManager.PixelTypeAtPosition(HeadPosition);

        if ((footPixel & headPixel).IsVoid())
        {
            LevelScreen.LemmingManager.RemoveLemming(this, LemmingRemovalReason.DeathVoid);
            return false;
        }

        return true;
    }

    private bool CheckTriggerAreas(
        in GadgetEnumerable gadgetsNearLemming,
        Span<Point> gadgetCheckPositions,
        bool isPostTeleportCheck)
    {
        if (isPostTeleportCheck)
        {
            _data.PreviousAnchorPosition = _data.AnchorPosition;
        }

        var result = CheckGadgets(in gadgetsNearLemming, gadgetCheckPositions) && LemmingManager.DoBlockerCheck(this);

        NextAction.TransitionLemmingToAction(this, false);

        return result;
    }

    private bool CheckGadgets(
        in GadgetEnumerable gadgetsNearLemming,
        Span<Point> gadgetCheckPositions)
    {
        if (gadgetsNearLemming.Count == 0)
            return true;

        var movementHelper = new LemmingMovementHelper(this, gadgetCheckPositions);
        var length = movementHelper.EvaluateCheckPositions();

        return CheckGadgetHitBoxCollisions(in gadgetsNearLemming, gadgetCheckPositions[..length]);
    }

    private bool CheckGadgetHitBoxCollisions(in GadgetEnumerable gadgetEnumerable, ReadOnlySpan<Point> intermediatePositions)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            var currentState = gadget.CurrentState;

            foreach (var anchorPosition in intermediatePositions)
            {
                var footPosition = CurrentAction.GetFootPosition(this, anchorPosition);
                if (!gadget.ContainsEitherPoint(_data.Orientation, anchorPosition, footPosition))
                    continue;

                var firstMatchingFilter = GetFirstMatchingLemmingFilter(currentState.Filters);
                if (firstMatchingFilter is null)
                    continue;

                var beforeAction = CurrentAction;
                HandleGadgetInteraction(gadget, firstMatchingFilter, anchorPosition);
                var afterAction = CurrentAction;

                if (beforeAction != afterAction)
                {
                    _data.AnchorPosition = anchorPosition;
                    _data.CurrentBounds = afterAction.GetLemmingBounds(this);

                    return false;
                }
            }
        }

        return true;
    }

    private LemmingHitBoxFilter? GetFirstMatchingLemmingFilter(ReadOnlySpan<LemmingHitBoxFilter> filters)
    {
        foreach (var filter in filters)
        {
            if (filter.MatchesLemming(this))
            {
                return filter;
            }
        }

        return null;
    }

    private void HandleGadgetInteraction(
        HitBoxGadget gadget,
        LemmingHitBoxFilter filter,
        Point checkPosition)
    {
        // If we're at the end of the check positions and Next action is not None
        // then transition. However, if NextAction is SplatterAction and there's water
        // at the position, the water takes precedence over splatting
        if (NextActionType != LemmingActionType.NoneAction &&
            checkPosition == _data.AnchorPosition &&
            (NextActionType != LemmingActionType.SplatterAction ||
            filter.HitBoxBehaviour != HitBoxInteractionType.Liquid))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (_data.JumpToHoistAdvance)
            {
                _data.AnimationFrame += 2;
                _data.PhysicsFrame += 2;
                _data.JumpToHoistAdvance = false;
            }

            NextAction = NoneAction.Instance;
        }

        gadget.OnLemmingHit(filter, this);
    }

    public void SetFastForwardTime(int fastForwardTime)
    {
        _data.FastForwardTime = fastForwardTime;
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    public void SetCountDownAction(uint countDownTimer, LemmingAction countDownAction, bool displayTimer)
    {
        _data.CountDownTimer = countDownTimer;
        _data.CountDownActionType = countDownAction.ActionType;

        Renderer.SetDisplayTimer(displayTimer);
    }

    public void ClearCountDownAction()
    {
        _data.CountDownTimer = 0;
        _data.CountDownActionType = LemmingActionType.NoneAction;
    }

    public void OnUpdatePosition()
    {
        Renderer.UpdatePosition();
    }

    public void OnRemoval(LemmingRemovalReason removalReason)
    {
        CurrentAction = NoneAction.Instance;
        Renderer.UpdateLemmingState(removalReason is LemmingRemovalReason.DeathExploder or LemmingRemovalReason.DeathStoner);
    }

    public unsafe void SetRawDataFromOther(Lemming otherLemming)
    {
        void* otherPointer = otherLemming._data.GetPointer();
        void* thisPointer = _data.GetPointer();
        CopyLemmingSnapshotBytes(otherPointer, thisPointer);
        OnSnapshotApplied();
    }

    private static unsafe void CopyLemmingSnapshotBytes(void* sourcePointer, void* destinationPointer)
    {
        var sourceSpan = new ReadOnlySpan<byte>(sourcePointer, LemmingData.SizeInBytes);
        var destinationSpan = new Span<byte>(destinationPointer, LemmingData.SizeInBytes);
        sourceSpan.CopyTo(destinationSpan);
    }

    public void SetRawData(Orientation orientation, FacingDirection facingDirection, int tribeId, uint rawStateData)
    {
        _data.Orientation = orientation;
        _data.FacingDirection = facingDirection;
        _data.TribeId = tribeId;
        _data.State = rawStateData;

        OnSnapshotApplied();
    }

    public void OnSnapshotApplied()
    {
        UpdateAllColors();
        Renderer.UpdateLemmingState(IsActive);
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
        LevelScreen.LemmingManager.UpdateZombieState(this);
    }

    public void SetTribeAffiliation(int tribeId)
    {
        _data.TribeId = tribeId;
        UpdateAllColors();
    }

    public void ClearAllPermanentSkills()
    {
        _data.State &= ~LemmingAbilityConstants.PermanentSkillBitMask;
        UpdateHairAndBodyColors();
    }

    private void UpdateAllColors()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_data.TribeId);

        if (HasPermanentSkill)
        {
            LemmingColors.HairColor = tribe.ColorData.PermanentSkillHairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.PermanentSkillBodyColor;
        }
        else
        {
            LemmingColors.HairColor = tribe.ColorData.HairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.BodyColor;
        }

        LemmingColors.SkinColor = IsZombie
            ? tribe.ColorData.ZombieSkinColor
            : tribe.ColorData.SkinColor;

        if (IsAcidLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.WaterLemmingFootColor;
        }
        else
        {
            LemmingColors.FootColor = LemmingColors.SkinColor;
        }

        LemmingColors.PaintColor = tribe.ColorData.PaintColor;
    }

    private void UpdateHairAndBodyColors()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_data.TribeId);

        if (HasPermanentSkill)
        {
            LemmingColors.HairColor = tribe.ColorData.PermanentSkillHairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.PermanentSkillBodyColor;
        }
        else
        {
            LemmingColors.HairColor = tribe.ColorData.HairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_data.TribeId);

        LemmingColors.SkinColor = IsZombie
            ? tribe.ColorData.ZombieSkinColor
            : tribe.ColorData.SkinColor;

        if (IsAcidLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.WaterLemmingFootColor;
        }
        else
        {
            LemmingColors.FootColor = LemmingColors.SkinColor;
        }
    }

    [DebuggerStepThrough]
    public bool Equals(Lemming? other)
    {
        var otherId = -1;
        if (other is not null) otherId = other.Id;
        return Id == otherId;
    }
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Lemming other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;

    [DebuggerStepThrough]
    public static bool operator ==(Lemming? left, Lemming? right)
    {
        var leftId = -1;
        if (left is not null) leftId = left.Id;
        var rightId = -1;
        if (right is not null) rightId = right.Id;
        return leftId == rightId;
    }
    [DebuggerStepThrough]
    public static bool operator !=(Lemming? left, Lemming? right) => !(left == right);
}
