using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering;

public sealed class ActionSprite : IDisposable
{
    public Point Size { get; }
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }
    public int NumberOfFrames { get; }

    public Texture2D Texture { get; }
    public Rectangle BoundingBox { get; }
    public Point AnchorPoint { get; }
    public int AnchorPointX { get; }
    public int AnchorPointY { get; }

    public ActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        Point anchorPoint)
    {
        Texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        Size = new Point(spriteWidth, spriteHeight);
        BoundingBox = new Rectangle(0, 0, spriteWidth, spriteHeight);
        NumberOfFrames = numberOfFrames;
        AnchorPoint = anchorPoint;
        AnchorPointX = anchorPoint.X;
        AnchorPointY = anchorPoint.Y;
    }

    public Rectangle GetSourceRectangleForFrame(int frame)
    {
        return new Rectangle(0, frame * SpriteHeight, SpriteWidth, SpriteHeight);
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}