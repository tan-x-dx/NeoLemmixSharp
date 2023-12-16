using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class EmptyLevelRegion : ILevelRegion
{
    public static readonly EmptyLevelRegion Instance = new();

    private EmptyLevelRegion()
    {
    }

    public bool ContainsPoint(LevelPosition levelPosition) => false;
}