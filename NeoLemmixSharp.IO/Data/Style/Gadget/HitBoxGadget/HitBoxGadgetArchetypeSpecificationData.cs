using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

[DebuggerDisplay("HitBox - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class HitBoxGadgetArchetypeSpecificationData : IGadgetArchetypeSpecificationData
{
    private readonly BitArrayDictionary<GadgetArchetypeMiscDataTypeHasher, BitBuffer32, GadgetArchetypeMiscDataType, int> _miscData = GadgetArchetypeMiscDataTypeHasher.CreateBitArrayDictionary<int>();

    public GadgetType GadgetType => GadgetType.HitBoxGadget;

    public required ResizeType ResizeType { get; init; }
    public required RectangularRegion NineSliceData { get; init; }

    public required HitBoxGadgetStateArchetypeData[] GadgetStates { get; init; }

    internal void AddMiscData(GadgetArchetypeMiscDataType miscDataType, int value) => _miscData.Add(miscDataType, value);
    public bool HasMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData.ContainsKey(miscDataType);
    public int GetMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData[miscDataType];
    public bool TryGetMiscData(GadgetArchetypeMiscDataType miscDataType, out int value) => _miscData.TryGetValue(miscDataType, out value);

    ReadOnlySpan<IGadgetStateArchetypeData> IGadgetArchetypeSpecificationData.AllStates => GadgetStates;
}

[DebuggerDisplay("{StateName}")]
public sealed class HitBoxGadgetStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required Point HitBoxOffset { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] TriggerBehaviourLinks { get; init; }
    public required HitBoxFilterData[] HitBoxFilters { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateArchetypeData.InnateTriggers => InnateTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateArchetypeData.InnateBehaviours => InnateBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateArchetypeData.TriggerBehaviourLinks => TriggerBehaviourLinks;
}
