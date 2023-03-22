using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.Rendering;

public sealed class ActionSprite : IDisposable
{
    private readonly LevelPosition _anchorPoint;
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }
    public int NumberOfFrames { get; }

    public Texture2D Texture { get; }
    public Rectangle BoundingBox { get; }
    public Rectangle GetBoundingBox() => new(0, 0, SpriteWidth, SpriteHeight);
    public LevelPosition GetAnchorPoint() => _anchorPoint;

    public ActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        Texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        BoundingBox = new Rectangle(0, 0, spriteWidth, spriteHeight);
        NumberOfFrames = numberOfFrames;

        _anchorPoint = anchorPoint;
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