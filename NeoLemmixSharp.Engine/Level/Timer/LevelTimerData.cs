using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public struct LevelTimerData
{
    public const int LevelTimerDataSize = 1 * sizeof(int);

    public int AdditionalSeconds;
}

public unsafe readonly struct LevelTimerDataPointer
{
    public readonly LevelTimerData* _pointer;

    public LevelTimerDataPointer(void* pointer) => _pointer = (LevelTimerData*)pointer;
    public LevelTimerDataPointer(nint pointerHandle) => _pointer = (LevelTimerData*)pointerHandle;

    public ref int AdditionalSeconds => ref Unsafe.AsRef<int>(&_pointer->AdditionalSeconds);
}
