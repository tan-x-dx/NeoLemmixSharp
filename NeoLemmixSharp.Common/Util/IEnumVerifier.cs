namespace NeoLemmixSharp.Common.Util;

public interface IEnumVerifier<TEnum>
    where TEnum : unmanaged, Enum
{
    static abstract TEnum GetEnumValue(int rawValue);
}
