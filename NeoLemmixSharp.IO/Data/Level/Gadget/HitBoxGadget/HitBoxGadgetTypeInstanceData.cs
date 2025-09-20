using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;

public sealed class HitBoxGadgetTypeInstanceData : IGadgetTypeInstanceData
{
    private readonly BitArrayDictionary<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType, int> _propertyLookup = new(new());

    public GadgetType GadgetType => GadgetType.HitBoxGadget;
    public required int InitialStateId { get; init; }
    public required HitBoxGadgetStateInstanceData[] GadgetStates { get; init; }
    public required GadgetLayerColorData[] LayerColorData { get; init; }

    public int GetProperty(GadgetPropertyType gadgetProperty) => _propertyLookup[gadgetProperty];
}
