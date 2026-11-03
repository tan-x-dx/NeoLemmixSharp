namespace NeoLemmixSharp.Common.Util.Rewind;

public interface IPointerData<T>
    where T : unmanaged, IPointerData<T>
{
    static abstract T Create(nint dataHandle);
    static abstract int SizeInBytes { get; }
}
