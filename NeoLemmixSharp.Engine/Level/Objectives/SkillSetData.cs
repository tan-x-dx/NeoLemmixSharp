using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public unsafe readonly struct SkillSetData
{
    public const int SkillSetDataSize = 3 * sizeof(int);

    [StructLayout(LayoutKind.Sequential, Size = SkillSetDataSize)]
    private struct SkillSetDataRaw
    {
        public int AdditionalQuantity;
        public int AmountUsed;
        public int CurrentSkillLimit;
    }

    private readonly SkillSetDataRaw* _data;

    public SkillSetData(void* pointer) => _data = (SkillSetDataRaw*)pointer;
    public SkillSetData(nint pointerHandle) => _data = (SkillSetDataRaw*)pointerHandle;

    public ref int AdditionalQuantity => ref Unsafe.AsRef<int>(&_data->AdditionalQuantity);
    public ref int AmountUsed => ref Unsafe.AsRef<int>(&_data->AmountUsed);
    public ref int CurrentSkillLimit => ref Unsafe.AsRef<int>(&_data->CurrentSkillLimit);
}
