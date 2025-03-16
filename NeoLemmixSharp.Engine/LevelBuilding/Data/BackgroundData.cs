using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class BackgroundData
{
    public required Color Color { get; init; }
    public required string BackgroundImageName { get; init; }

    public bool IsSolidColor => string.IsNullOrWhiteSpace(BackgroundImageName);
}