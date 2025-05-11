using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class BackgroundData
{
    public required Color Color { get; init; }
    public required string BackgroundImageName { get; init; }

    public bool IsSolidColor => string.IsNullOrWhiteSpace(BackgroundImageName);
}