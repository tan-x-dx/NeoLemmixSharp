namespace NeoLemmixSharp.Common.Util;

public interface IPointerData<T>
    where T : unmanaged, IPointerData<T>
{
    static abstract T Create(nint dataRef);
    static abstract int SizeInBytes { get; }
}
