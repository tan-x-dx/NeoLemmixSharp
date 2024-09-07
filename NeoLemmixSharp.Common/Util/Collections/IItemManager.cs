namespace NeoLemmixSharp.Common.Util.Collections;

public interface IItemManager<T> : IPerfectHasher<T>
    where T : class
{
    ReadOnlySpan<T> AllItems { get; }
}