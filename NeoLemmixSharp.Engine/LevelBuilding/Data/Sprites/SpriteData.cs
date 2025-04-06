using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class SpriteData
{
    public required Texture2D Texture { get; init; }

    public required Size SpriteSize { get; init; }

    public int NumberOfLayers => FrameCountsPerLayer.Length;
    public required int[] FrameCountsPerLayer { get; init; }
}
