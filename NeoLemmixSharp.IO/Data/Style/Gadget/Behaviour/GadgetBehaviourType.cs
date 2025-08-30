using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

public enum GadgetBehaviourType
{
    None,

    GadgetOutputSignal,
    GadgetChangeInternalState,
    GadgetMoveFree,
    GadgetMoveUntilPosition,
    GadgetResizeFree,
    GadgetConstrainedResize,

    GadgetAnimationRenderLayer,
    GadgetAnimationSetFrame,
    GadgetAnimationIncrementFrame,
    GadgetAnimationDecrementFrame,

    LemmingBehaviour,

    GlobalAdditionalTime,
    GlobalSkillCountChange,
}

public readonly struct GadgetBehaviourTypeHasher : IEnumIdentifierHelper<GadgetBehaviourTypeHasher.GadgetBehaviourTypeBitBuffer, GadgetBehaviourType>
{
    private const int NumberOfEnumValues = 14;
    public static GadgetBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetBehaviourType>(rawValue, NumberOfEnumValues);

    public int NumberOfItems => NumberOfEnumValues;
    public int Hash(GadgetBehaviourType item) => (int)item;
    public GadgetBehaviourType UnHash(int index) => (GadgetBehaviourType)index;
    public void CreateBitBuffer(out GadgetBehaviourTypeBitBuffer buffer) => buffer = new();

    [InlineArray(GadgetBehaviourTypeBitBufferLength)]
    public struct GadgetBehaviourTypeBitBuffer : IBitBuffer
    {
        private const int GadgetBehaviourTypeBitBufferLength = (NumberOfEnumValues + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => GadgetBehaviourTypeBitBufferLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, GadgetBehaviourTypeBitBufferLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, GadgetBehaviourTypeBitBufferLength);
    }
}
