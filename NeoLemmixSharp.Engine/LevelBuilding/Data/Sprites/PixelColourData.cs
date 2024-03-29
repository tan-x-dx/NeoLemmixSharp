﻿using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public readonly struct PixelColorData
{
    public readonly int Width;
    public readonly int Height;

    private readonly uint[] _colorData;

    public static PixelColorData GetPixelColorDataFromTexture(Texture2D texture)
    {
        var width = texture.Width;
        var height = texture.Height;
        var data = new uint[width * height];

        texture.GetData(data);

        return new PixelColorData(width, height, data);
    }

    public PixelColorData(int width, int height, uint[] colorData)
    {
        Width = width;
        Height = height;
        _colorData = colorData;
    }

    public uint this[int x, int y]
    {
        get
        {
            var i = Width * y + x;

            return _colorData[i];
        }
        set
        {
            var i = Width * y + x;

            _colorData[i] = value;
        }
    }

    public Texture2D CreateTexture(GraphicsDevice graphicsDevice)
    {
        var result = new Texture2D(graphicsDevice, Width, Height);
        result.SetData(_colorData);
        return result;
    }
}