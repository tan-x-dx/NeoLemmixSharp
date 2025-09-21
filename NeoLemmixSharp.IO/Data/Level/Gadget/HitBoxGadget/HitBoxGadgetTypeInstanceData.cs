using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;

public sealed class HitBoxGadgetTypeInstanceData : IGadgetTypeInstanceData
{
    private readonly BitArrayDictionary<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType, int> _propertyLookup = new(new());

    public GadgetType GadgetType => GadgetType.HitBoxGadget;
    public required int InitialStateId { get; init; }
    public required HitBoxGadgetStateInstanceData[] GadgetStates { get; init; }

    public int GetProperty(GadgetPropertyType gadgetProperty) => _propertyLookup[gadgetProperty];
}

[DebuggerDisplay("{OverrideStateName}")]
public sealed class HitBoxGadgetStateInstanceData : IGadgetStateInstanceData
{
    public required GadgetStateName OverrideStateName { get; init; }
    public required GadgetTriggerData[] CustomTriggers { get; init; }
    public required GadgetBehaviourData[] CustomBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] CustomTriggerBehaviourLinks { get; init; }
    public required HitBoxFilterData[] CustomHitBoxFilters { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateInstanceData.CustomTriggers => CustomTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateInstanceData.CustomBehaviours => CustomBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateInstanceData.CustomTriggerBehaviourLinks => CustomTriggerBehaviourLinks;
}
