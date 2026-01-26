using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public readonly unsafe struct HatchGroupData : IPointerData<HatchGroupData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HatchGroupData Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => HatchGroupDataSize;

    private const int HatchGroupDataSize = 4 * sizeof(int);

    [StructLayout(LayoutKind.Sequential, Size = HatchGroupDataSize)]
    private struct HatchGroupDataRaw
    {
        public int HatchIndex;
        public uint NextLemmingCountDown;
        public int LemmingsToRelease;
        public uint CurrentSpawnInterval;
    }

    private readonly HatchGroupDataRaw* _data;

    private HatchGroupData(nint pointerHandle) => _data = (HatchGroupDataRaw*)pointerHandle;

    public ref int HatchIndex => ref Unsafe.AsRef<int>(&_data->HatchIndex);
    public ref uint NextLemmingCountDown => ref Unsafe.AsRef<uint>(&_data->NextLemmingCountDown);
    public ref int LemmingsToRelease => ref Unsafe.AsRef<int>(&_data->LemmingsToRelease);
    public ref uint CurrentSpawnInterval => ref Unsafe.AsRef<uint>(&_data->CurrentSpawnInterval);
}
