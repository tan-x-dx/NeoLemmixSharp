using NeoLemmixSharp.Common.Util.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Identity;

public sealed class ExtendedEnumTypeComparer<T> :
	IEqualityComparer<T>,
	IEquatable<ExtendedEnumTypeComparer<T>>,
	IComparer<T>,
	IPerfectHasher<T>
	where T : class, IExtendedEnumType<T>
{
	public static readonly ExtendedEnumTypeComparer<T> Instance = new();

	private ExtendedEnumTypeComparer()
	{
	}

	public bool Equals(T? x, T? y)
	{
		if (x is null) return y is null;
		if (y is null) return false;
		return x.Id == y.Id;
	}

	public int GetHashCode(T obj) => 2965019 * obj.Id +
									 5477821;

	public int Compare(T? x, T? y) => IdEquatableItemHelperMethods.Compare(x, y);

	public bool Equals(ExtendedEnumTypeComparer<T>? other) => other is not null;
	public override bool Equals(object? obj) => obj is ExtendedEnumTypeComparer<T>;
	public override int GetHashCode() => typeof(T).GetHashCode();

	public int NumberOfItems => T.NumberOfItems;

	public int Hash(T item) => item.Id;
	public T UnHash(int index) => T.AllItems[index];

	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Dictionary<T, TValue> CreateDictionary<TValue>() => new(Instance);

	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SimpleSet<T> CreateSimpleSet()
	{
		return new SimpleSet<T>(Instance);
	}
}