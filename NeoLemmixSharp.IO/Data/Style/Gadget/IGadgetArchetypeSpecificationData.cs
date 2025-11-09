using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetArchetypeSpecificationData
{
    GadgetType GadgetType { get; }

    ReadOnlySpan<IGadgetStateArchetypeData> AllStates { get; }
}
