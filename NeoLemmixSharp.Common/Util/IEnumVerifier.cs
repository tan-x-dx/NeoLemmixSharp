namespace NeoLemmixSharp.Common.Util;

public interface IEnumVerifier<TEnum>
    where TEnum : unmanaged, Enum
{
    TEnum GetEnumValue(int rawValue);
}
