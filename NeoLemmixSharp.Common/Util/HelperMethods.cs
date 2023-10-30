namespace NeoLemmixSharp.Common.Util;

public static class HelperMethods
{
    public static void DisposeOf<T>(ref T obj)
        where T : class, IDisposable
    {
        obj.Dispose();
        obj = null!;
    }
}