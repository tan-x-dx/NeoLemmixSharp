using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingAbilityCriterion : LemmingCriterion
{
    private readonly ILemmingAbilityChanger[] _allowedLemmingAbilties;
    private readonly uint[] _requiredAbilties;

    public LemmingAbilityCriterion(LemmingAbilitySet? allowedAbilties, LemmingAbilitySet? disallowedAbilties)
        : base(LemmingCriteriaType.RequiredLemmingAbility)
    {
        if (allowedAbilties is null)
        {
            if (disallowedAbilties is null)
                throw new ArgumentNullException("Both input sets are null!");

            InitialiseSingleSet(disallowedAbilties, false, out _allowedLemmingAbilties, out _requiredAbilties);
        }
        else if (disallowedAbilties is null)
        {
            InitialiseSingleSet(allowedAbilties, true, out _allowedLemmingAbilties, out _requiredAbilties);
        }
        else
        {
            InitialiseBothSets(allowedAbilties, disallowedAbilties, out _allowedLemmingAbilties, out _requiredAbilties);
        }
    }

    private static void InitialiseSingleSet(LemmingAbilitySet states, bool isRequired, out ILemmingAbilityChanger[] allowedLemmingAbilties, out uint[] requiredAbilties)
    {
        Debug.Assert(states.Count > 0);

        allowedLemmingAbilties = states.ToArray();
        requiredAbilties = BitArrayHelpers.CreateBitArray(states.Count, isRequired);
    }

    private static void InitialiseBothSets(
        LemmingAbilitySet allowedAbilties,
        LemmingAbilitySet disallowedAbilties,
        out ILemmingAbilityChanger[] allowedLemmingAbilties,
        out uint[] requiredAbilties)
    {
        allowedAbilties.ExceptWith(disallowedAbilties);

        var numberOfAbilties = allowedAbilties.Count + disallowedAbilties.Count;
        Debug.Assert(numberOfAbilties > 0);

        allowedLemmingAbilties = new ILemmingAbilityChanger[numberOfAbilties];
        requiredAbilties = BitArrayHelpers.CreateBitArray(numberOfAbilties, true);

        var stateSpan = Helpers.CreateSpan(allowedLemmingAbilties, 0, allowedAbilties.Count);
        allowedAbilties.CopyTo(stateSpan);
        stateSpan = Helpers.CreateSpan(allowedLemmingAbilties, allowedAbilties.Count, disallowedAbilties.Count);
        disallowedAbilties.CopyTo(stateSpan);

        var bitsSpan = new Span<uint>(requiredAbilties);

        for (var i = allowedAbilties.Count; i < numberOfAbilties; i++)
        {
            BitArrayHelpers.ClearBit(bitsSpan, i);
        }
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        for (int i = 0; i < _allowedLemmingAbilties.Length; i++)
        {
            var lemmingAbility = _allowedLemmingAbilties[i];
            var lemmingHasAbility = lemmingAbility.LemmingHasAbility(lemming);
            var requiredValue = BitArrayHelpers.GetBit(_requiredAbilties, i);

            if (lemmingHasAbility != requiredValue)
                return false;
        }

        return true;
    }
}
