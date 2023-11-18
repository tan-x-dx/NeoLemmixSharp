using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class EmptyLevelRegion : ILevelRegion
{
    public static EmptyLevelRegion Instance { get; } = new();

    private EmptyLevelRegion()
    {
    }

    public bool ContainsPoint(LevelPosition levelPosition) => false;
}