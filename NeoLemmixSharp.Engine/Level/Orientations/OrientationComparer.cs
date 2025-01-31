using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public readonly struct OrientationComparer : IPerfectHasher<Orientation>
{
    public int NumberOfItems => EngineConstants.NumberOfOrientations;

    [Pure]
    public int Hash(Orientation item) => item.RotNum;
    [Pure]
    public Orientation UnHash(int index) => new(index);

    [Pure]
    public static OrientationSet CreateSimpleSet(bool fullSet = false) => new(new OrientationComparer(), fullSet);
    [Pure]
    public static SimpleDictionary<OrientationComparer, Orientation, TValue> CreateSimpleDictionary<TValue>() => new(new OrientationComparer());
}
