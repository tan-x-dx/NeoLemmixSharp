using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Rewind;
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

    public LemmingRenderer Renderer => LevelScreenRenderer.Instance.LemmingRendererFor(Id);

    #region Lemming Data

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

    private ref PackedBytes32 State => ref _data.State;
    public ref int TribeId => ref _data.TribeId;

    RectangularRegion IRectangularBounds.CurrentBounds => _data.CurrentBounds;
    public ref RectangularRegion CurrentBounds => ref _data.CurrentBounds;

    public LemmingActionType PreviousActionType => _data.PreviousActionType;
    public ref LemmingActionType CurrentActionType => ref _data.CurrentActionType;
    public ref LemmingActionType NextActionType => ref _data.NextActionType;
    public ref LemmingActionType CountDownActionType => ref _data.CountDownActionType;

    public ref Point DehoistPin => ref _data.DehoistPin;
    public ref Point LaserHitLevelPosition => ref _data.LaserHitLevelPosition;
    public ref Point AnchorPosition => ref _data.AnchorPosition;
    public ref Point PreviousAnchorPosition => ref _data.PreviousAnchorPosition;

    public bool IsStartingAction
    {
        get => _data.IsStartingAction;
        set => _data.IsStartingAction = value;
    }
    public bool ConstructivePositionFreeze
    {
        get => _data.ConstructivePositionFreeze;
        set => _data.ConstructivePositionFreeze = value;
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

    public ref byte AnimationFrame => ref _data.AnimationFrame;
    public ref byte PhysicsFrame => ref _data.PhysicsFrame;
    public ref byte NumberOfBricksLeft => ref _data.NumberOfBricksLeft;
    public ref byte AscenderProgress => ref _data.AscenderProgress;
    public ref byte DisarmingFrames => ref _data.DisarmingFrames;
    public ref byte JumpProgress => ref _data.JumpProgress;
    public ref byte ParticleTimer => ref _data.ParticleTimer;

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

    public bool HasPermanentSkill => (State.GetRawValue() & LemmingAbilityConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (State.GetRawValue() & LemmingAbilityConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (State.GetRawValue() & LemmingAbilityConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(State.GetRawValue() & LemmingAbilityConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT removed and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (LemmingRemovalReason == LemmingRemovalReason.None) &&
                                         ((State.GetRawValue() & LemmingAbilityConstants.AssignableSkillBitMask) == (1U << LemmingAbilityConstants.ActiveBitIndex));

    public bool IsClimber
    {
        get => State.GetBit(LemmingAbilityConstants.ClimberBitIndex);
        set
        {
            State.SetBit(LemmingAbilityConstants.ClimberBitIndex, value);
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => State.GetBit(LemmingAbilityConstants.FloaterBitIndex);
        set
        {
            ref var states = ref State;
            if (value)
            {
                states.SetBit(LemmingAbilityConstants.FloaterBitIndex);
                states.ClearBit(LemmingAbilityConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                states.ClearBit(LemmingAbilityConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => State.GetBit(LemmingAbilityConstants.GliderBitIndex);
        set
        {
            ref var states = ref State;
            if (value)
            {
                states.SetBit(LemmingAbilityConstants.GliderBitIndex);
                states.ClearBit(LemmingAbilityConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                states.ClearBit(LemmingAbilityConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => State.GetBit(LemmingAbilityConstants.SliderBitIndex);
        set
        {
            State.SetBit(LemmingAbilityConstants.SliderBitIndex, value);
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => State.GetBit(LemmingAbilityConstants.SwimmerBitIndex);
        set
        {
            ref var states = ref State;
            if (value)
            {
                states.SetBit(LemmingAbilityConstants.SwimmerBitIndex);
                // Deliberately knock out the acid/water lemmings
                states.ClearBit(LemmingAbilityConstants.AcidLemmingBitIndex);
                states.ClearBit(LemmingAbilityConstants.WaterLemmingBitIndex);
            }
            else
            {
                states.ClearBit(LemmingAbilityConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => State.GetBit(LemmingAbilityConstants.DisarmerBitIndex);
        set
        {
            State.SetBit(LemmingAbilityConstants.DisarmerBitIndex, value);
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => State.GetBit(LemmingAbilityConstants.PermanentFastForwardBitIndex);
        set
        {
            State.SetBit(LemmingAbilityConstants.PermanentFastForwardBitIndex, value);
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
        }
    }

    public bool IsAcidLemming
    {
        get => State.GetBit(LemmingAbilityConstants.AcidLemmingBitIndex);
        set
        {
            ref var states = ref State;
            if (value)
            {
                states.SetBit(LemmingAbilityConstants.AcidLemmingBitIndex);
                // Deliberately knock out the swimmer/water lemmings
                states.ClearBit(LemmingAbilityConstants.SwimmerBitIndex);
                states.ClearBit(LemmingAbilityConstants.WaterLemmingBitIndex);
            }
            else
            {
                states.ClearBit(LemmingAbilityConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => State.GetBit(LemmingAbilityConstants.WaterLemmingBitIndex);
        set
        {
            ref var states = ref State;
            if (value)
            {
                states.SetBit(LemmingAbilityConstants.WaterLemmingBitIndex);
                // Deliberately knock out the swimmer/acid lemmings
                states.ClearBit(LemmingAbilityConstants.SwimmerBitIndex);
                states.ClearBit(LemmingAbilityConstants.AcidLemmingBitIndex);
            }
            else
            {
                states.ClearBit(LemmingAbilityConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => State.GetBit(LemmingAbilityConstants.ActiveBitIndex);
        private set => State.SetBit(LemmingAbilityConstants.ActiveBitIndex, value);
    }

    public bool IsNeutral
    {
        get => State.GetBit(LemmingAbilityConstants.NeutralBitIndex);
        set
        {
            State.SetBit(LemmingAbilityConstants.NeutralBitIndex, value);
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => State.GetBit(LemmingAbilityConstants.ZombieBitIndex);
        set
        {
            if (IsZombie == value)
                return;

            State.SetBit(LemmingAbilityConstants.ZombieBitIndex, value);
            LevelScreen.LemmingManager.UpdateZombieState(this);
            UpdateSkinColor();
        }
    }

    #endregion

    public static Lemming CreatePrePlacedLemming(
        DataHandle dataHandle,
        int id,
        IO.Data.Level.LemmingInstanceData prototype)
    {
        var result = new Lemming(dataHandle, id);

        result._data.AnchorPosition = prototype.Position;
        result._data.CurrentActionType = prototype.InitialLemmingActionType;
        result._data.DihedralTransformation = prototype.DihedralTransformation;
        result._data.TribeId = prototype.TribeId;
        result._data.State = prototype.State;

        return result;
    }

    public static Lemming CreateHatchLemming(
        DataHandle dataHandle,
        int id)
    {
        var result = new Lemming(dataHandle, id);

        result._data.CurrentActionType = LemmingActionType.NoneAction;

        return result;
    }

    public static Lemming CreateSimulationLemming(DataHandle dataHandle)
    {
        return new Lemming(dataHandle, EngineConstants.SimulationLemmingId);
    }

    private Lemming(DataHandle dataHandle, int id)
    {
        _data = dataHandle.CreateItem<LemmingData>();
        Id = id;
    }

    public void Initialise()
    {
        IsActive = true;
        PreviousAnchorPosition = AnchorPosition;

        var initialActionType = CurrentActionType;

        if (initialActionType == LemmingActionType.NoneAction)
        {
            initialActionType = LemmingActionType.WalkerAction;
        }

        CurrentBounds = initialActionType.GetLemmingBounds(this);

        LemmingAction.TransitionLemmingToAction(this, false, initialActionType);

        UpdateAllColors();

        Renderer.UpdateLemmingState(true);
    }

    [SkipLocalsInit]
    public void Tick()
    {
        _data.PreviousActionType = CurrentActionType;
        // No transition to do at the end of lemming movement
        NextActionType = LemmingActionType.NoneAction;

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

    public void ResetFlagsOnNewAction() => _data.ResetFlagsOnNewAction();

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

        byte frame = AnimationFrame;
        frame++;
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

        frame = PhysicsFrame;
        frame++;
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

            NextActionType = LemmingActionType.NoneAction;
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
        CountDownActionType = countDownActionType;
    }

    public void ClearCountDownAction()
    {
        CountDownTimer = 0;
        CountDownActionType = LemmingActionType.NoneAction;
    }

    public void OnUpdatePosition()
    {
        Renderer.UpdatePosition();
    }

    public void OnRemoval(LemmingRemovalReason removalReason)
    {
        LemmingRemovalReason = removalReason;
        CurrentActionType = LemmingActionType.NoneAction;
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
