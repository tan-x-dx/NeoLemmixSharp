using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Util;

public static class HelperMethods
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
    /// <param name="dictionary">The dictionary to dispose of</param>
    public static void DisposeOf<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : class, IDisposable
    {
        foreach (var value in dictionary.Values)
        {
            value.Dispose();
        }

        dictionary.Clear();
    }

    public static Vector2 GetSize(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }
}