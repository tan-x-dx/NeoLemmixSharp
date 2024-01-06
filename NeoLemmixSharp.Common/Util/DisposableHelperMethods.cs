namespace NeoLemmixSharp.Common.Util;

public static class DisposableHelperMethods
{
	/// <summary>
	/// Disposes of the object and then sets the reference to null
	/// </summary>
	/// <typeparam name="T">The type of the object to dispose</typeparam>
	/// <param name="obj">The object to dispose</param>
	public static void DisposeOf<T>(ref T obj)
		where T : class, IDisposable
	{
		obj.Dispose();
		obj = null!;
	}

	/// <summary>
	/// Disposes of all values in the dictionary and then clears the dictionary
	/// </summary>
	/// <typeparam name="TKey">The key type of the dictionary</typeparam>
	/// <typeparam name="TValue">The value type of the dictionary - the items to be disposed of</typeparam>
	/// <param name="dictionary">The dictionary to dispose the contents of</param>
	public static void DisposeOfAll<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
		where TKey : notnull
		where TValue : class, IDisposable
	{
		foreach (var value in dictionary.Values)
		{
			value.Dispose();
		}

		dictionary.Clear();
	}

	/// <summary>
	/// Disposes of all values in the span
	/// </summary>
	/// <typeparam name="T">The value type of the span - the items to be disposed of</typeparam>
	/// <param name="values">The span to dispose the contents of</param>
	public static void DisposeOfAll<T>(ReadOnlySpan<T> values)
		where T : class, IDisposable
	{
		foreach (var value in values)
		{
			value.Dispose();
		}
	}
}