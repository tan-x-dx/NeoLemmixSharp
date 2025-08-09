namespace NeoLemmixSharp.Common.Util.Collections;

public interface IItemManager<T>
    where T : class
{
    ReadOnlySpan<T> AllItems { get; }
}
