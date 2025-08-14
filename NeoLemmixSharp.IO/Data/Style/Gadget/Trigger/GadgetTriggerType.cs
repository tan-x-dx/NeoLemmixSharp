using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

public enum GadgetTriggerType
{
    AlwaysTrue,

    GadgetLinkTrigger,

    GadgetAnimationFinished,

    LemmingHitBoxTrigger
}

public readonly struct GadgetTriggerTypeHasher : IEnumIdentifierHelper<GadgetTriggerTypeHasher.GadgetTriggerTypeBitBuffer, GadgetTriggerType>
{
    private const int NumberOfEnumValues = 10;
    public static GadgetTriggerType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetTriggerType>(rawValue, NumberOfEnumValues);

    public int NumberOfItems => NumberOfEnumValues;
    public int Hash(GadgetTriggerType item) => (int)item;
    public GadgetTriggerType UnHash(int index) => (GadgetTriggerType)index;
    public void CreateBitBuffer(out GadgetTriggerTypeBitBuffer buffer) => buffer = new();

    [InlineArray(GadgetTriggerTypeBitBufferLength)]
    public struct GadgetTriggerTypeBitBuffer : IBitBuffer
    {
        private const int GadgetTriggerTypeBitBufferLength = (NumberOfEnumValues + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => GadgetTriggerTypeBitBufferLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, GadgetTriggerTypeBitBufferLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, GadgetTriggerTypeBitBufferLength);
    }
}
