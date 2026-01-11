using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public unsafe readonly struct HatchGroupData
{
    public const int HatchGroupDataSize = 4 * sizeof(int);

    [StructLayout(LayoutKind.Sequential, Size = HatchGroupDataSize)]
    private struct HatchGroupDataRaw
    {
        public int HatchIndex;
        public uint NextLemmingCountDown;
        public int LemmingsToRelease;
        public uint CurrentSpawnInterval;
    }

    private readonly HatchGroupDataRaw* _data;

    public HatchGroupData(void* pointer) => _data = (HatchGroupDataRaw*)pointer;
    public HatchGroupData(nint pointerHandle) => _data = (HatchGroupDataRaw*)pointerHandle;

    public ref int HatchIndex => ref Unsafe.AsRef<int>(&_data->HatchIndex);
    public ref uint NextLemmingCountDown => ref Unsafe.AsRef<uint>(&_data->NextLemmingCountDown);
    public ref int LemmingsToRelease => ref Unsafe.AsRef<int>(&_data->LemmingsToRelease);
    public ref uint CurrentSpawnInterval => ref Unsafe.AsRef<uint>(&_data->CurrentSpawnInterval);
}
