using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class BackgroundData
{
    public required bool IsSolidColor { get; init; }
    public required Color Color { get; init; }
    public required string BackgroundImageName { get; init; }
}