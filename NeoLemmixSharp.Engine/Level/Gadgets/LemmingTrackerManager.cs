using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Numerics.Tensors;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public readonly unsafe struct LemmingTrackerManager
{
    private readonly nint _lemmingTrackerDataHandle;
    private readonly int _length;

    public LemmingTrackerManager(nint dataHandle, int lengthInBytes)
    {
        _lemmingTrackerDataHandle = dataHandle;
        Debug.Assert((lengthInBytes & 7) == 0);
        _length = lengthInBytes / sizeof(uint);
    }

    public void Tick()
    {
        var x = Helpers.CreateReadOnlySpan<uint>((void*)_lemmingTrackerDataHandle, _length);
        var destination = Helpers.CreateSpan<uint>((void*)_lemmingTrackerDataHandle, _length);

        TensorPrimitives.ShiftLeft(x, 1, destination);
        TensorPrimitives.BitwiseAnd(x, LemmingTracker.LemmingTrackerTickMask, destination);
    }
}
