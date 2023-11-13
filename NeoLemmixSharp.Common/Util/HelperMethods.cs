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

    public static Vector2 GetSize(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }
}