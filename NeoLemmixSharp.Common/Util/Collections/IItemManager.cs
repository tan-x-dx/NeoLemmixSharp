namespace NeoLemmixSharp.Common.Util.Collections;

public interface IItemManager<T>
    where T : class
{
    int NumberOfItems { get; }
    ReadOnlySpan<T> AllItems { get; }
}