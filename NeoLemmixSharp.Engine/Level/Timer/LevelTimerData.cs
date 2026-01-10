using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public unsafe readonly struct LevelTimerData
{
    public const int LevelTimerDataSize = 1 * sizeof(int);

    private struct LevelTimerDataRaw
    {
        public int AdditionalSeconds;
    }

    private readonly LevelTimerDataRaw* _data;

    public LevelTimerData(void* pointer) => _data = (LevelTimerDataRaw*)pointer;
    public LevelTimerData(nint pointerHandle) => _data = (LevelTimerDataRaw*)pointerHandle;

    public ref int AdditionalSeconds => ref Unsafe.AsRef<int>(&_data->AdditionalSeconds);
}
