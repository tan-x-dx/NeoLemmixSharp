using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.LevelIo.Writing;

public interface ISectionIdentifierHelper<TEnum> : IPerfectHasher<TEnum>, IBitBufferCreator<BitBuffer32>
    where TEnum : unmanaged, Enum
{
    static abstract TEnum GetEnumValue(uint rawValue);
}
