using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public struct LemmingManagerData
{
    public const int LemmingManagerDataSize = 6 * sizeof(int);

    public int NumberOfLemmingsReleasedFromHatch;
    public int NumberOfClonedLemmings;

    public int LemmingsToRelease;
    public int LemmingsOut;
    public int LemmingsRemoved;
    public int LemmingsSaved;
}

public unsafe readonly struct LemmingManagerDataPointer
{
    private readonly LemmingManagerData* _pointer;

    public void* GetPointer() => _pointer;

    public LemmingManagerDataPointer(void* pointer) => _pointer = (LemmingManagerData*)pointer;
    public LemmingManagerDataPointer(nint pointerHandle) => _pointer = (LemmingManagerData*)pointerHandle;

    public ref int NumberOfLemmingsReleasedFromHatch => ref Unsafe.AsRef<int>(&_pointer->NumberOfLemmingsReleasedFromHatch);
    public ref int NumberOfClonedLemmings => ref Unsafe.AsRef<int>(&_pointer->NumberOfClonedLemmings);

    public ref int LemmingsToRelease => ref Unsafe.AsRef<int>(&_pointer->LemmingsToRelease);
    public ref int LemmingsOut => ref Unsafe.AsRef<int>(&_pointer->LemmingsOut);
    public ref int LemmingsRemoved => ref Unsafe.AsRef<int>(&_pointer->LemmingsRemoved);
    public ref int LemmingsSaved => ref Unsafe.AsRef<int>(&_pointer->LemmingsSaved);
}
