namespace NeoLemmixSharp.Common.Util.Identity;

public interface IExtendedEnumType<T> : IIdEquatable<T>
    where T : class, IExtendedEnumType<T>
{
    static abstract int NumberOfItems { get; }
    static abstract ReadOnlySpan<T> AllItems { get; }
}