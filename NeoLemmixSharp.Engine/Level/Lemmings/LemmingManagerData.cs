using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingManagerData : IPointerData<LemmingManagerData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingManagerData Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => LemmingManagerDataSize;

    private const int LemmingManagerDataSize = 6 * sizeof(int);

    [StructLayout(LayoutKind.Sequential, Size = LemmingManagerDataSize)]
    private struct LemmingManagerDataRaw
    {
        public int NumberOfLemmingsReleasedFromHatch;
        public int NumberOfClonedLemmings;

        public int LemmingsToRelease;
        public int LemmingsOut;
        public int LemmingsRemoved;
        public int LemmingsSaved;
    }

    private readonly LemmingManagerDataRaw* _data;

    private LemmingManagerData(nint pointerHandle) => _data = (LemmingManagerDataRaw*)pointerHandle;

    public ref int NumberOfLemmingsReleasedFromHatch => ref Unsafe.AsRef<int>(&_data->NumberOfLemmingsReleasedFromHatch);
    public ref int NumberOfClonedLemmings => ref Unsafe.AsRef<int>(&_data->NumberOfClonedLemmings);

    public ref int LemmingsToRelease => ref Unsafe.AsRef<int>(&_data->LemmingsToRelease);
    public ref int LemmingsOut => ref Unsafe.AsRef<int>(&_data->LemmingsOut);
    public ref int LemmingsRemoved => ref Unsafe.AsRef<int>(&_data->LemmingsRemoved);
    public ref int LemmingsSaved => ref Unsafe.AsRef<int>(&_data->LemmingsSaved);
}
