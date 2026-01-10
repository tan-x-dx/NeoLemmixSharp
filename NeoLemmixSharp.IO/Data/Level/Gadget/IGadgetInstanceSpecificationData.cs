using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

public interface IGadgetInstanceSpecificationData
{
    GadgetType GadgetType { get; }

    int CalculateExtraNumberOfBytesNeededForSnapshotting();
}
