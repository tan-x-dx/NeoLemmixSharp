using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class SpriteData
{
    public required Texture2D Texture { get; init; }

    public required int SpriteWidth { get; init; }
    public required int SpriteHeight { get; init; }

    public required int NumberOfLayers { get; init; }
    public required int NumberOfFrames { get; init; }
}