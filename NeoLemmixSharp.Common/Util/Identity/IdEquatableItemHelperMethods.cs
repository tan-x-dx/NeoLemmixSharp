﻿namespace NeoLemmixSharp.Common.Util.Identity;

public static class IdEquatableItemHelperMethods
{
	public static void ValidateUniqueIds<T>(ReadOnlySpan<T> items)
		where T : class, IIdEquatable<T>
	{
		if (items.Length == 0)
			return;

		var maxActionId = int.MinValue;
		var minActionId = int.MaxValue;
		var allItemIds = new HashSet<int>();

		foreach (var item in items)
		{
			var id = item.Id;

			minActionId = Math.Min(minActionId, id);
			maxActionId = Math.Max(maxActionId, id);

			if (!allItemIds.Add(id))
			{
				var typeName = typeof(T).Name;

				throw new Exception($"Duplicated {typeName} ID: {id}");
			}
		}

		if (minActionId != 0 || maxActionId != items.Length - 1)
		{
			var typeName = typeof(T).Name;
			throw new Exception($"{typeName} ids do not span a full set of values from 0 - {items.Length - 1}");
		}
	}

	public static int Compare<T>(T? x, T? y)
		where T : class, IIdEquatable<T>
	{
		if (ReferenceEquals(x, y)) return 0;
		if (x is null) return -1;
		if (y is null) return 1;

		return x.Id.CompareTo(y.Id);
	}
}