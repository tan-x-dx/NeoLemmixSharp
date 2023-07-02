using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Rendering.Level.Viewport;

public readonly struct SpriteLayerData
{
    public readonly int SpriteX;
    public readonly int SpriteY;

    public readonly Color SpriteColor;

    public SpriteLayerData(int spriteX, int spriteY, Color spriteColor)
    {
        SpriteX = spriteX;
        SpriteY = spriteY;
        SpriteColor = spriteColor;
    }
}