﻿using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.LevelBuilding.Sprites;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Rendering.Level.Viewport;

public sealed class SpriteDrawingData
{
    private readonly int _originalSpriteWidth;
    private readonly int _originalSpriteHeight;

    private readonly PixelColourData _colourData;
    public DihedralTransformation DihedralTransformation { get; }
    public int ThisSpriteWidth { get; }
    public int ThisSpriteHeight { get; }

    public SpriteDrawingData(
        Orientation orientation,
        FacingDirection facingDirection,
        int originalSpriteWidth,
        int originalSpriteHeight,
        int numberOfFrames,
        int numberOfLayers)
    {
        _originalSpriteWidth = originalSpriteWidth;
        _originalSpriteHeight = originalSpriteHeight;

        var rotNum = orientation.RotNum;

        if ((rotNum & 1) == 0)
        {
            ThisSpriteWidth = _originalSpriteWidth;
            ThisSpriteHeight = _originalSpriteHeight;
        }
        else
        {
            ThisSpriteWidth = _originalSpriteHeight;
            ThisSpriteHeight = _originalSpriteWidth;
        }

        var uints = new uint[originalSpriteWidth * originalSpriteHeight * numberOfFrames * numberOfLayers];
        _colourData = new PixelColourData(ThisSpriteWidth * numberOfLayers, ThisSpriteHeight * numberOfFrames, uints);
        DihedralTransformation = DihedralTransformation.GetForTransformation(facingDirection == LeftFacingDirection.Instance, rotNum);
    }

    public void Set(uint pixel, int x0, int y0, int layer, int frame)
    {
        DihedralTransformation.Transform(
            x0,
            y0,
            _originalSpriteWidth - 1,
            _originalSpriteHeight - 1,
            out var x1,
            out var y1);

        var x2 = x1 + ThisSpriteWidth * layer;
        var y2 = y1 + ThisSpriteHeight * frame;
        _colourData.Set(x2, y2, pixel);
    }

    public Texture2D ToTexture(GraphicsDevice graphicsDevice)
    {
        var result = new Texture2D(graphicsDevice, _colourData.Width, _colourData.Height);
        result.SetData(_colourData.ColourData);
        return result;
    }
}