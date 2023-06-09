﻿using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Io.LevelReading.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Sprites;

public sealed class SpriteDrawingData
{
    private readonly int _originalSpriteWidth;
    private readonly int _originalSpriteHeight;

    private readonly PixelColourData _colourData;
    public Orientation Orientation { get; }
    public DihedralTransformation DihedralTransformation { get; }
    public int ThisSpriteWidth { get; }
    public int ThisSpriteHeight { get; }

    public SpriteDrawingData(
        Orientation orientation,
        int originalSpriteWidth,
        int originalSpriteHeight,
        int numberOfFrames)
    {
        Orientation = orientation;
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

        var uints = new uint[originalSpriteWidth * originalSpriteHeight * numberOfFrames];
        _colourData = new PixelColourData(ThisSpriteWidth, ThisSpriteHeight * numberOfFrames, uints);
        DihedralTransformation = DihedralTransformation.GetForTransformation(false, rotNum);
    }

    public void Set(uint pixel, int x0, int y0, int frame)
    {
        DihedralTransformation.Transform(x0,
            y0,
            _originalSpriteWidth - 1,
            _originalSpriteHeight - 1, out var x1, out var y1);

        var y2 = y1 + ThisSpriteHeight * frame;
        _colourData.Set(x1, y2, pixel);
    }

    public Texture2D ToTexture(GraphicsDevice graphicsDevice)
    {
        var result = new Texture2D(graphicsDevice, _colourData.Width, _colourData.Height);
        result.SetData(_colourData.ColourData);
        return result;
    }
}