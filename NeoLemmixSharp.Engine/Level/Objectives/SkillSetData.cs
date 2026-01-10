using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public struct SkillSetData
{
    public const int SkillSetDataSize = 3 * sizeof(int);

    public int AdditionalQuantity;
    public int AmountUsed;
    public int CurrentSkillLimit;
}

public unsafe readonly struct SkillSetDataPointer
{
    private readonly SkillSetData* _pointer;

    public SkillSetDataPointer(void* pointer) => _pointer = (SkillSetData*)pointer;
    public SkillSetDataPointer(nint pointerHandle) => _pointer = (SkillSetData*)pointerHandle;

    public ref int AdditionalQuantity => ref Unsafe.AsRef<int>(&_pointer->AdditionalQuantity);
    public ref int AmountUsed => ref Unsafe.AsRef<int>(&_pointer->AmountUsed);
    public ref int CurrentSkillLimit => ref Unsafe.AsRef<int>(&_pointer->CurrentSkillLimit);
}
