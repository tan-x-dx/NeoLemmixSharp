using System.Numerics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IIdEquatable<T> : IEquatable<T>, IEqualityOperators<T, T, bool>
    where T : class, IIdEquatable<T>
{
    int Id { get; }
}