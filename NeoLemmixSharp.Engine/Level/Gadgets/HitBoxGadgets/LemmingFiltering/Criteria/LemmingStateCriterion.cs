using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingStateCriterion : LemmingCriterion
{
    private readonly ILemmingState[] _allowedLemmingStates;
    private readonly uint[] _requiredStates;

    public LemmingStateCriterion(LemmingStateSet? allowedStates, LemmingStateSet? disallowedStates)
        : base(LemmingCriteriaType.RequiredLemmingState)
    {
        if (allowedStates is null)
        {
            if (disallowedStates is null)
                throw new ArgumentException("Both input sets are null!");

            InitialiseSingleSet(disallowedStates, false, out _allowedLemmingStates, out _requiredStates);
        }
        else if (disallowedStates is null)
        {
            InitialiseSingleSet(allowedStates, true, out _allowedLemmingStates, out _requiredStates);
        }
        else
        {
            InitialiseBothSets(allowedStates, disallowedStates, out _allowedLemmingStates, out _requiredStates);
        }
    }

    private static void InitialiseSingleSet(LemmingStateSet states, bool isRequired, out ILemmingState[] allowedLemmingStates, out uint[] requiredStates)
    {
        Debug.Assert(states.Count > 0);

        allowedLemmingStates = states.ToArray();
        requiredStates = BitArrayHelpers.CreateBitArray(states.Count, isRequired);
    }

    private static void InitialiseBothSets(
        LemmingStateSet allowedStates,
        LemmingStateSet disallowedStates,
        out ILemmingState[] allowedLemmingStates,
        out uint[] requiredStates)
    {
        allowedStates.ExceptWith(disallowedStates);

        var numberOfStates = allowedStates.Count + disallowedStates.Count;
        Debug.Assert(numberOfStates > 0);

        allowedLemmingStates = new ILemmingState[numberOfStates];
        requiredStates = BitArrayHelpers.CreateBitArray(numberOfStates, true);

        var stateSpan = Helpers.CreateSpan(allowedLemmingStates, 0, allowedStates.Count);
        allowedStates.CopyTo(stateSpan);
        stateSpan = Helpers.CreateSpan(allowedLemmingStates, allowedStates.Count, disallowedStates.Count);
        disallowedStates.CopyTo(stateSpan);

        var bitsSpan = new Span<uint>(requiredStates);

        for (var i = allowedStates.Count; i < numberOfStates; i++)
        {
            BitArrayHelpers.ClearBit(bitsSpan, i);
        }
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        for (int i = 0; i < _allowedLemmingStates.Length; i++)
        {
            var lemmingState = _allowedLemmingStates[i];
            var stateIsApplied = lemmingState.IsApplied(lemming.State);
            var requiredValue = BitArrayHelpers.GetBit(_requiredStates, i);

            if (stateIsApplied != requiredValue)
                return false;
        }

        return true;
    }
}
