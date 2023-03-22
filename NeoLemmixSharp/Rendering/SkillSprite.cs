﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;

namespace NeoLemmixSharp.Rendering;

public sealed class SkillSprite : NeoLemmixSprite
{
    private readonly Texture2D _texture;
    private readonly LevelPosition _anchorPoint;
    public int SpriteWidth{get;}
    public int SpriteHeight { get; }
    public int NumberOfFrames { get; }
    public IOrientation Orientation { get; }
    public IFacingDirection FacingDirection { get; }

    public override Texture2D GetTexture() => _texture;
    public override Rectangle GetBoundingBox() => new(0, 0, SpriteWidth, SpriteHeight);
    public override LevelPosition GetAnchorPoint() => _anchorPoint;

    public override bool ShouldRender => false;

    public SkillSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint,
        IOrientation orientation,
        IFacingDirection facingDirection)
    {
        _texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        NumberOfFrames = numberOfFrames;

        _anchorPoint = anchorPoint;
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
    }

    public Rectangle GetSourceRectangleForFrame(int frame)
    {
        return new Rectangle(0, (frame % NumberOfFrames) * SpriteHeight, SpriteWidth, SpriteHeight);
    }
}