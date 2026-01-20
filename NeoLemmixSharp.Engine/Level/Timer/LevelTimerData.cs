using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public readonly unsafe struct LevelTimerData : IPointerData<LevelTimerData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelTimerData Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => LevelTimerDataSize;

    private const int LevelTimerDataSize = 1 * sizeof(int);

    [StructLayout(LayoutKind.Sequential, Size = LevelTimerDataSize)]
    private struct LevelTimerDataRaw
    {
        public int AdditionalSeconds;
    }

    private readonly LevelTimerDataRaw* _data;

    private LevelTimerData(nint pointerHandle) => _data = (LevelTimerDataRaw*)pointerHandle;

    public ref int AdditionalSeconds => ref Unsafe.AsRef<int>(&_data->AdditionalSeconds);
}
