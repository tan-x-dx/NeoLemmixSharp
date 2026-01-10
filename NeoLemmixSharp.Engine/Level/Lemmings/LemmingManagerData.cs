using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public unsafe readonly struct LemmingManagerData
{
    public const int LemmingManagerDataSize = 6 * sizeof(int);

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

    public LemmingManagerData(void* pointer) => _data = (LemmingManagerDataRaw*)pointer;
    public LemmingManagerData(nint pointerHandle) => _data = (LemmingManagerDataRaw*)pointerHandle;

    public ref int NumberOfLemmingsReleasedFromHatch => ref Unsafe.AsRef<int>(&_data->NumberOfLemmingsReleasedFromHatch);
    public ref int NumberOfClonedLemmings => ref Unsafe.AsRef<int>(&_data->NumberOfClonedLemmings);

    public ref int LemmingsToRelease => ref Unsafe.AsRef<int>(&_data->LemmingsToRelease);
    public ref int LemmingsOut => ref Unsafe.AsRef<int>(&_data->LemmingsOut);
    public ref int LemmingsRemoved => ref Unsafe.AsRef<int>(&_data->LemmingsRemoved);
    public ref int LemmingsSaved => ref Unsafe.AsRef<int>(&_data->LemmingsSaved);
}
