using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();

    private EmptyHitBoxRegion()
    {
    }

    public bool ContainsPoint(LevelPosition levelPosition) => false;
}