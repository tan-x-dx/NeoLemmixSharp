using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IEquatable<Lemming>, IRectangularBounds
{
    private readonly LemmingData _data;

    public int Id { get; }

    private LemmingColorData _colors = new();

    public LemmingColorData Colors => _colors;

    private LemmingRenderer Renderer => LevelScreenRenderer.Instance.LemmingRendererFor(Id);

    #region Lemming Data

    public LemmingActionType PreviousActionType => _data.PreviousActionType;

    public LemmingActionType CurrentActionType => _data.CurrentActionType;
    public void SetCurrentActionType(LemmingActionType actionType)
    {
        _data.CurrentActionType = actionType;
        Renderer.UpdateLemmingState(true);
    }

    public LemmingActionType NextActionType => _data.NextActionType;
    public void SetNextActionType(LemmingActionType actionType)
    {
        _data.NextActionType = actionType;
    }

    public LemmingActionType CountDownActionType => _data.CountDownActionType;

    public DihedralTransformation DihedralTransformation => _data.DihedralTransformation;

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

    RectangularRegion IRectangularBounds.CurrentBounds => _data.CurrentBounds;
    public ref RectangularRegion CurrentBounds => ref _data.CurrentBounds;

    public ref Point DehoistPin => ref _data.DehoistPin;
    public ref Point LaserHitLevelPosition => ref _data.LaserHitLevelPosition;
    public ref Point AnchorPosition => ref _data.AnchorPosition;
    public ref Point PreviousAnchorPosition => ref _data.PreviousAnchorPosition;

    public bool ConstructivePositionFreeze
    {
        get => _data.ConstructivePositionFreeze;
        set => _data.ConstructivePositionFreeze = value;
    }
    public bool IsStartingAction
    {
        get => _data.IsStartingAction;
        set => _data.IsStartingAction = value;
    }
    public bool PlacedBrick
    {
        get => _data.PlacedBrick;
        set => _data.PlacedBrick = value;
    }
    public bool StackLow
    {
        get => _data.StackLow;
        set => _data.StackLow = value;
    }
    public bool InitialFall
    {
        get => _data.InitialFall;
        set => _data.InitialFall = value;
    }
    public bool EndOfAnimation
    {
        get => _data.EndOfAnimation;
        set => _data.EndOfAnimation = value;
    }
    public bool LaserHit
    {
        get => _data.LaserHit;
        set => _data.LaserHit = value;
    }
    public bool JumpToHoistAdvance
    {
        get => _data.JumpToHoistAdvance;
        set => _data.JumpToHoistAdvance = value;
    }
    public int AnimationFrame
    {
        get => _data.AnimationFrame;
        set => _data.AnimationFrame = value;
    }
    public int PhysicsFrame
    {
        get => _data.PhysicsFrame;
        set => _data.PhysicsFrame = value;
    }
    public int NumberOfBricksLeft
    {
        get => _data.NumberOfBricksLeft;
        set => _data.NumberOfBricksLeft = value;
    }
    public int AscenderProgress
    {
        get => _data.AscenderProgress;
        set => _data.AscenderProgress = value;
    }
    public int DisarmingFrames
    {
        get => _data.DisarmingFrames;
        set => _data.DisarmingFrames = value;
    }
    public int JumpProgress
    {
        get => _data.JumpProgress;
        set => _data.JumpProgress = value;
    }
    public int ParticleTimer
    {
        get => _data.ParticleTimer;
        set => _data.ParticleTimer = value;
    }

    public ref int DistanceFallen => ref _data.DistanceFallen;
    public ref int TrueDistanceFallen => ref _data.TrueDistanceFallen;
    public ref int LaserRemainTime => ref _data.LaserRemainTime;
    public ref uint FastForwardTime => ref _data.FastForwardTime;
    public ref uint CountDownTimer => ref _data.CountDownTimer;
    public ref LemmingRemovalReason LemmingRemovalReason => ref _data.LemmingRemovalReason;

    public bool IsSimulation => Id == EngineConstants.SimulationLemmingId;
    public bool IsFastForward
    {
        get
        {
            uint fastForwardAmount = IsPermanentFastForwards ? 1U : 0U;
            fastForwardAmount |= FastForwardTime;

            return fastForwardAmount > 0;
        }
    }

    public Point HeadPosition => Orientation.MoveUp(AnchorPosition, 6);
    public Point FootPosition => CurrentActionType.GetFootPosition(DihedralTransformation, AnchorPosition);
    public Point CenterPosition => Orientation.MoveUp(AnchorPosition, 4);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<Point> GetJumperPositions() => _data.GetJumperPositions();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> CountDownCharBuffer() => _data.GetCountDownCharsSpan();

    #endregion

    #region Lemming Ability Properties

    public bool HasPermanentSkill => (_data.State & LemmingAbilityConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_data.State & LemmingAbilityConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_data.State & LemmingAbilityConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_data.State & LemmingAbilityConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT removed and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (LemmingRemovalReason == LemmingRemovalReason.None) && ((_data.State & LemmingAbilityConstants.AssignableSkillBitMask) == (1U << LemmingAbilityConstants.ActiveBitIndex));

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
        private set
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
    }

    public void Initialise()
    {
        IsActive = true;
        PreviousAnchorPosition = AnchorPosition;

        var initialActionType = CurrentActionType;
        CurrentBounds = initialActionType.GetLemmingBounds(this);

        if (initialActionType == LemmingActionType.NoneAction)
        {
            initialActionType = LemmingActionType.WalkerAction;
        }

        LemmingAction.TransitionLemmingToAction(this, false, initialActionType);

        UpdateAllColors();

        Renderer.UpdateLemmingState(true);
    }

    [SkipLocalsInit]
    public void Tick()
    {
        _data.PreviousActionType = CurrentActionType;
        // No transition to do at the end of lemming movement
        SetNextActionType(LemmingActionType.NoneAction);

        UpdateParticleTimer();
        UpdateCountDownTimer();
        UpdateFastForwardTimer();

        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the region enclosed by a box centred around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var p1 = Orientation.Move(AnchorPosition, new(-5, 12));
        var p2 = Orientation.Move(AnchorPosition, new(5, -12));

        var checkPositionsBounds = new RectangularRegion(p1, p2);

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        Span<Point> gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];

        EvaluateLemmingLogic(in gadgetsNearLemming, gadgetCheckPositions);
    }

    private void EvaluateLemmingLogic(
        in GadgetEnumerable gadgetsNearLemming,
        Span<Point> gadgetCheckPositions)
    {
        if (!EvaluateLemmingAction(in gadgetsNearLemming)) return;
        if (!CheckLevelBoundaries()) return;
        if (!CheckTriggerAreas(in gadgetsNearLemming, gadgetCheckPositions, false)) return;
        if (IsZombie) return;
        if (!LevelScreen.LemmingManager.AnyZombies()) return;

        LevelScreen.LemmingManager.DoZombieCheck(this);
    }

    [SkipLocalsInit]
    public void Simulate(bool checkGadgets)
    {
        if (!IsSimulation)
            ThrowSimulationLemmingException();

        UpdateParticleTimer();
        UpdateCountDownTimer();
        UpdateFastForwardTimer();

        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the region enclosed by a box centred around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var p1 = Orientation.Move(AnchorPosition, new(-5, 12));
        var p2 = Orientation.Move(AnchorPosition, new(5, -12));

        var checkPositionsBounds = new RectangularRegion(p1, p2);

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        var handleGadgets = EvaluateLemmingAction(in gadgetsNearLemming) && CheckLevelBoundaries() && checkGadgets;
        if (handleGadgets)
        {
            Span<Point> gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];

            CheckTriggerAreas(in gadgetsNearLemming, gadgetCheckPositions, false);
        }
    }

    private void UpdateParticleTimer()
    {
        var particleTimer = ParticleTimer;
        if (particleTimer > 0)
        {
            particleTimer--;
            ParticleTimer = particleTimer;
        }
    }

    private void UpdateCountDownTimer()
    {
        var countDownTimer = CountDownTimer;
        if (countDownTimer == 0)
            return;

        countDownTimer--;
        CountDownTimer = countDownTimer;

        var countDownValue = IsFastForward
                ? (countDownTimer + EngineConstants.EngineTicksPerSecond - 1) / EngineConstants.EngineTicksPerSecond
                : (countDownTimer + EngineConstants.GameplayTicksPerSecond - 1) / EngineConstants.GameplayTicksPerSecond;
        var textSpan = _data.GetCountDownCharsSpan();
        NumberFormattingHelpers.WriteDigits(textSpan, countDownValue);

        if (countDownTimer == 0)
            OhNoerAction.HandleCountDownTransition(this);
    }

    private void UpdateFastForwardTimer()
    {
        uint fastForwardTime = FastForwardTime;

        if (fastForwardTime == 0)
            return;

        fastForwardTime--;
        FastForwardTime = fastForwardTime;

        if (fastForwardTime == 0)
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    /// <summary>
    /// Handle one frame of the lemming's action, and update data accordingly.
    /// Some actions (e.g. exiter, splatter, etc) may result in the lemming being killed off in some way.
    /// In that case, the method returns false - no extra work is necessary (don't bother checking gadgets, for example).
    /// </summary>
    /// <returns>True if more work needs to be done this frame</returns>
    private bool EvaluateLemmingAction(in GadgetEnumerable gadgetsNearLemming)
    {
        var currentActionType = CurrentActionType;
        //var currentAction = CurrentAction;

        var frame = AnimationFrame + 1;
        var actionFrames = LemmingAction.GetNumberOfAnimationFramesForActionType(currentActionType);
        if (frame == actionFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentActionType == LemmingActionType.FloaterAction ||
                currentActionType == LemmingActionType.GliderAction)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }
        }
        AnimationFrame = frame;

        frame = PhysicsFrame + 1;
        actionFrames = LemmingAction.GetMaxPhysicsFramesForActionType(currentActionType);
        if (frame == actionFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentActionType == LemmingActionType.FloaterAction ||
                currentActionType == LemmingActionType.GliderAction)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }

            EndOfAnimation = LemmingAction.IsOneTimeAction(currentActionType);
        }

        PhysicsFrame = frame;
        PreviousAnchorPosition = AnchorPosition;

        var result = LemmingAction.UpdateLemming(this, in gadgetsNearLemming, currentActionType);
        CurrentBounds = currentActionType.GetLemmingBounds(this);

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
            PreviousAnchorPosition = AnchorPosition;
        }

        bool result = gadgetsNearLemming.Count == 0 || (CheckGadgets(in gadgetsNearLemming, gadgetCheckPositions) && LemmingManager.DoBlockerCheck(this));
        LemmingAction.TransitionLemmingToAction(this, false, NextActionType);

        return result;
    }

    private bool CheckGadgets(
        in GadgetEnumerable gadgetsNearLemming,
        Span<Point> gadgetCheckPositions)
    {
        var movementHelper = new LemmingMovementHelper(this, gadgetCheckPositions);
        var length = movementHelper.EvaluateCheckPositions();

        return CheckGadgetHitBoxCollisions(in gadgetsNearLemming, gadgetCheckPositions.SliceUnsafe(0, length));
    }

    private bool CheckGadgetHitBoxCollisions(in GadgetEnumerable gadgetEnumerable, ReadOnlySpan<Point> intermediatePositions)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            var currentState = gadget.CurrentState;

            foreach (var anchorPosition in intermediatePositions)
            {
                var footPosition = FootPosition;
                if (!gadget.ContainsEitherPoint(Orientation, anchorPosition, footPosition))
                    continue;

                var firstMatchingFilter = GetFirstMatchingLemmingFilter(currentState.Filters);
                if (firstMatchingFilter is null)
                    continue;

                var beforeAction = CurrentActionType;
                EvaluateGadgetInteraction(gadget, firstMatchingFilter, anchorPosition);
                var afterAction = CurrentActionType;

                if (beforeAction != afterAction)
                {
                    AnchorPosition = anchorPosition;
                    CurrentBounds = afterAction.GetLemmingBounds(this);

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

    private void EvaluateGadgetInteraction(
        HitBoxGadget gadget,
        LemmingHitBoxFilter filter,
        Point checkPosition)
    {
        // If we're at the end of the check positions and Next action is not None
        // then transition. However, if NextAction is SplatterAction and there's water
        // at the position, the water takes precedence over splatting
        if (NextActionType != LemmingActionType.NoneAction &&
            checkPosition == AnchorPosition &&
            (NextActionType != LemmingActionType.SplatterAction ||
            filter.HitBoxBehaviour != HitBoxInteractionType.Liquid))
        {
            LemmingAction.TransitionLemmingToAction(this, false, NextActionType);
            if (JumpToHoistAdvance)
            {
                AnimationFrame += 2;
                PhysicsFrame += 2;
                JumpToHoistAdvance = false;
            }

            SetNextActionType(LemmingActionType.NoneAction);
        }

        gadget.OnLemmingHit(filter, this);
    }

    public void SetFastForwardTime(uint fastForwardTime)
    {
        FastForwardTime = fastForwardTime;
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    public void SetCountDownAction(uint countDownTimer, LemmingActionType countDownActionType)
    {
        CountDownTimer = countDownTimer;
        _data.CountDownActionType = countDownActionType;
    }

    public void ClearCountDownAction()
    {
        CountDownTimer = 0;
        _data.CountDownActionType = LemmingActionType.NoneAction;
    }

    public void OnUpdatePosition()
    {
        Renderer.UpdatePosition();
    }

    public void OnRemoval(LemmingRemovalReason removalReason)
    {
        LemmingRemovalReason = removalReason;
        SetCurrentActionType(LemmingActionType.NoneAction);
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

    public void SetRawData(DihedralTransformation dihedralTransformation, int tribeId, uint rawStateData)
    {
        _data.DihedralTransformation = dihedralTransformation;
        _data.TribeId = tribeId;
        _data.State = rawStateData;

        OnSnapshotApplied();
    }

    public void OnSnapshotApplied()
    {
        UpdateAllColors();
        var renderer = Renderer;
        renderer.UpdateLemmingState(IsActive);
        renderer.ResetPosition();
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
            _colors.HairColor = tribe.ColorData.PermanentSkillHairColor;
            _colors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.PermanentSkillBodyColor;
        }
        else
        {
            _colors.HairColor = tribe.ColorData.HairColor;
            _colors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.BodyColor;
        }

        _colors.SkinColor = IsZombie
            ? tribe.ColorData.ZombieSkinColor
            : tribe.ColorData.SkinColor;

        if (IsAcidLemming)
        {
            _colors.FootColor = tribe.ColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            _colors.FootColor = tribe.ColorData.WaterLemmingFootColor;
        }
        else
        {
            _colors.FootColor = _colors.SkinColor;
        }

        _colors.PaintColor = tribe.ColorData.PaintColor;
    }

    private void UpdateHairAndBodyColors()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_data.TribeId);

        if (HasPermanentSkill)
        {
            _colors.HairColor = tribe.ColorData.PermanentSkillHairColor;
            _colors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.PermanentSkillBodyColor;
        }
        else
        {
            _colors.HairColor = tribe.ColorData.HairColor;
            _colors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_data.TribeId);

        _colors.SkinColor = IsZombie
            ? tribe.ColorData.ZombieSkinColor
            : tribe.ColorData.SkinColor;

        if (IsAcidLemming)
        {
            _colors.FootColor = tribe.ColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            _colors.FootColor = tribe.ColorData.WaterLemmingFootColor;
        }
        else
        {
            _colors.FootColor = _colors.SkinColor;
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

    [DoesNotReturn]
    private static void ThrowSimulationLemmingException() => throw new InvalidOperationException("Use simulation lemming for simulations!");
}
