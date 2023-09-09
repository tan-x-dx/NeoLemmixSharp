namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IIdEquatable<T> : IEquatable<T>
    where T : class, IIdEquatable<T>
{
    int Id { get; }

    static abstract bool operator ==(T left, T right);
    static abstract bool operator !=(T left, T right);
}