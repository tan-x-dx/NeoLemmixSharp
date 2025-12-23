using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;

public ref struct LemmingCriteriaBuilder
{
    private readonly TribeManager _tribeManager;
    private readonly Orientation _instanceOrientation;
    private readonly FacingDirection _instanceFacingDirection;

    private OrientationSet? _orientationSet = null;
    private int _facingDirectionIds = 0;
    private LemmingActionSet? _lemmingActionSet = null;
    private LemmingStateSet? _allowedLemmingStateSet = null;
    private LemmingStateSet? _disallowedLemmingStateSet = null;
    private TribeSet? _tribeSet = null;
    private int _numberOfCriteria = 0;
    private bool _hasRequiredStates = false;
    private bool _hasDisallowedStates = false;

    public LemmingCriteriaBuilder(TribeManager tribeManager, Orientation instanceOrientation, FacingDirection facingDirection)
    {
        _tribeManager = tribeManager;
        _instanceOrientation = instanceOrientation;
        _instanceFacingDirection = facingDirection;
    }

    public LemmingCriterion[] BuildLemmingCriteria(ReadOnlySpan<HitBoxCriteriaData> hitBoxCriteriaData)
    {
        foreach (var hitBoxCriteriaDatum in hitBoxCriteriaData)
        {
            RegisterLemmingCriterion(hitBoxCriteriaDatum);
        }

        CheckSetIsNotFull(ref _orientationSet);
        // Have to deal with facing directions differently since there's
        // only two possible values. It's not worth creating a set object
        // for just two values so we mess around with IDs directly.
        if (_facingDirectionIds == 3)
        {
            _facingDirectionIds = 0;
            _numberOfCriteria--;
        }

        CheckSetIsNotFull(ref _lemmingActionSet);
        CheckSetIsNotFull(ref _tribeSet);

        return CreateLemmingCriteriaArray();
    }

    private void RegisterLemmingCriterion(HitBoxCriteriaData hitBoxCriteriaDatum)
    {
        switch (hitBoxCriteriaDatum.LemmingCriteria)
        {
            case LemmingCriteriaType.LemmingOrientation:
                AddOrientationToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            case LemmingCriteriaType.LemmingFacingDirection:
                AddFacingDirectionToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            case LemmingCriteriaType.LemmingAction:
                AddLemmingActionToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            case LemmingCriteriaType.RequiredLemmingState:
                AddRequiredLemmingStateToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            case LemmingCriteriaType.DisallowedLemmingState:
                AddDisallowedLemmingStateToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            case LemmingCriteriaType.LemmingTribe:
                AddLemmingTribeToCriteria(hitBoxCriteriaDatum.ItemId);
                break;

            default:
                Helpers.ThrowUnknownEnumValueException<LemmingCriteriaType, LemmingCriteriaType>(hitBoxCriteriaDatum.LemmingCriteria);
                break;
        }
    }

    private readonly LemmingCriterion[] CreateLemmingCriteriaArray()
    {
        var numberOfCriteria = _numberOfCriteria;
        numberOfCriteria += (_hasRequiredStates | _hasDisallowedStates) ? 1 : 0;

        var result = Helpers.GetArrayForSize<LemmingCriterion>(numberOfCriteria);
        var i = 0;

        if (_orientationSet is not null) result[i++] = new LemmingOrientationCriterion(_orientationSet);
        if (_facingDirectionIds != 0)
        {
            var id = _facingDirectionIds == 1 ? EngineConstants.RightFacingDirectionId : EngineConstants.LeftFacingDirectionId;
            result[i++] = LemmingFacingDirectionCriterion.ForFacingDirection(id);
        }
        if (_lemmingActionSet is not null) result[i++] = new LemmingActionCriterion(_lemmingActionSet);
        if (_hasRequiredStates | _hasDisallowedStates) result[i++] = new LemmingStateCriterion(_allowedLemmingStateSet, _disallowedLemmingStateSet);
        if (_tribeSet is not null) result[i++] = new LemmingTribeCriterion(_tribeSet);

        Debug.Assert(i == result.Length);

        return result;
    }

    private void AddOrientationToCriteria(int itemId)
    {
        if (_orientationSet is null)
        {
            _orientationSet = Orientation.CreateBitArraySet();
            _numberOfCriteria++;
        }

        var orientation = new Orientation(itemId + _instanceOrientation.RotNum);
        _orientationSet.Add(orientation);
    }

    private void AddFacingDirectionToCriteria(int itemId)
    {
        var newFacingDirectionId = (itemId ^ _instanceFacingDirection.Id) & 1;

        if (_facingDirectionIds == 0)
        {
            _numberOfCriteria++;
        }

        _facingDirectionIds |= 1 << newFacingDirectionId;
    }

    private void AddLemmingActionToCriteria(int itemId)
    {
        if (_lemmingActionSet is null)
        {
            _lemmingActionSet = LemmingAction.CreateBitArraySet();
            _numberOfCriteria++;
        }

        var lemmingAction = LemmingAction.GetActionOrDefault(itemId);
        _lemmingActionSet.Add(lemmingAction);
    }

    private void AddRequiredLemmingStateToCriteria(int itemId)
    {
        if (_allowedLemmingStateSet is null)
        {
            _allowedLemmingStateSet = ILemmingState.CreateBitArraySet();
            _hasRequiredStates = true;
        }

        var lemmingState = ILemmingState.AllItems[itemId];
        _allowedLemmingStateSet.Add(lemmingState);
    }

    private void AddDisallowedLemmingStateToCriteria(int itemId)
    {
        if (_disallowedLemmingStateSet is null)
        {
            _disallowedLemmingStateSet = ILemmingState.CreateBitArraySet();
            _hasDisallowedStates = true;
        }

        var lemmingState = ILemmingState.AllItems[itemId];
        _disallowedLemmingStateSet.Add(lemmingState);
    }

    private void AddLemmingTribeToCriteria(int itemId)
    {
        if (_tribeSet is null)
        {
            _tribeSet = _tribeManager.CreateBitArraySet();
            _numberOfCriteria++;
        }

        var tribe = _tribeManager.GetTribe(itemId);
        _tribeSet.Add(tribe);
    }

    // If the set contains all possible values, then it is functionally useless.
    // All queries will always return true, so we might as well not even bother
    // creating a criterion object.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckSetIsNotFull<TPerfectHasher, TBuffer, T>(ref BitArraySet<TPerfectHasher, TBuffer, T>? set)
        where TPerfectHasher : IBitBufferCreator<TBuffer, T>
        where TBuffer : struct, IBitBuffer
        where T : notnull
    {
        if (set is not null && set.IsFull)
        {
            set = null;
            _numberOfCriteria--;
        }
    }
}
