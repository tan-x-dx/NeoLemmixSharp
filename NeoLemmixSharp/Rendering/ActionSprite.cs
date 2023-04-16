using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Rendering;

public sealed class ActionSprite : IDisposable
{
    public LevelPosition Size { get; }
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }
    public int NumberOfFrames { get; }

    public Texture2D Texture { get; }
    public LevelPosition AnchorPoint { get; }

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
        Size = new LevelPosition(spriteWidth, spriteHeight);
        NumberOfFrames = numberOfFrames;
        AnchorPoint = anchorPoint;
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