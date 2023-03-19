using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.LevelBuilding;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Rendering;

public sealed class SpriteBank : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly Dictionary<string, Texture2D> _textureLookup = new();
    private readonly Dictionary<string, SkillSprite> _skillSpriteLookup = new();

    public SpriteBank(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        CreateAnchorTexture();
    }

    private void CreateAnchorTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 3, 3);

        var red = new Color(200, 0, 0, 255);
        var yellow = new Color(200, 200, 0, 255);

        var x = new uint[9];
        x[1] = red.PackedValue;
        x[3] = red.PackedValue;
        x[4] = yellow.PackedValue;
        x[5] = red.PackedValue;
        x[7] = red.PackedValue;
        anchorTexture.SetData(x);
        _textureLookup.Add("anchor", anchorTexture);
    }

    public void ProcessLemmingStateTexture(string stateName, LemmingSpriteData spriteData, Texture2D texture)
    {
        var spriteWidth = texture.Width / 2;
        var spriteHeight = texture.Height / spriteData.NumberOfFrames;

        var originalPixelColourData = PixelColourData.GetPixelColourDataFromTexture(texture);

        ProcessLefts(stateName, spriteData, originalPixelColourData);
        ProcessRights(stateName, spriteData, originalPixelColourData);
    }

    private void ProcessLefts(string stateName, LemmingSpriteData spriteData, PixelColourData originalPixelColourData)
    {
        CreateSprites(
            stateName,
            LeftFacingDirection.Instance,
            spriteData,
            originalPixelColourData,
            0,
            spriteData.LeftFootX,
            spriteData.LeftFootY);
    }

    private void ProcessRights(string stateName, LemmingSpriteData spriteData, PixelColourData originalPixelColourData)
    {
        CreateSprites(
            stateName,
            RightFacingDirection.Instance,
            spriteData,
            originalPixelColourData,
            originalPixelColourData.Width / 2,
            spriteData.RightFootX,
            spriteData.RightFootY);
    }

    private void CreateSprites(
        string stateName,
        IFacingDirection facingDirection,
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        int dx0,
        int footX,
        int footY)
    {
        var spriteWidth = originalPixelColourData.Width / 2;
        var spriteHeight = originalPixelColourData.Height / spriteData.NumberOfFrames;

        var spriteDrawingDatas = IOrientation
            .AllOrientations
            .Select(o => new SpriteDrawingData(o, spriteWidth, spriteHeight, spriteData.NumberOfFrames))
            .ToArray();

        for (var f = 0; f < spriteData.NumberOfFrames; f++)
        {
            for (var x0 = 0; x0 < spriteWidth; x0++)
            {
                for (var y0 = 0; y0 < spriteHeight; y0++)
                {
                    var pixel = originalPixelColourData.Get(x0 + dx0, y0 + f * spriteHeight);

                    for (var i = 0; i < spriteDrawingDatas.Length; i++)
                    {
                        spriteDrawingDatas[i].Set(pixel, x0, y0, f);
                    }
                }
            }
        }

        foreach (var spriteDrawingData in spriteDrawingDatas)
        {
            var f = facingDirection;
            if ((spriteDrawingData.Orientation.RotNum & 1) == 1)
            {
                f = f.OppositeDirection;
            }

            var key = $"{stateName}_{f}_{spriteDrawingData.Orientation}";

            var texture = spriteDrawingData.ToTexture(_graphicsDevice);

            spriteDrawingData.DihedralTransformation.Transform(
                spriteWidth - 1,
                spriteHeight - 1,
                footX,
                footY,
                out var footX1,
                out var footY1);

            var skillSprite = new SkillSprite(
                texture,
                spriteDrawingData.ThisSpriteWidth,
                spriteDrawingData.ThisSpriteHeight,
                spriteData.NumberOfFrames,
                new LevelPosition(footX1, footY1),
                spriteDrawingData.Orientation,
                facingDirection);
            _skillSpriteLookup.Add(key, skillSprite);
        }
    }

    public void Dispose()
    {
        foreach (var texture in _skillSpriteLookup.Values)
        {
            texture.Dispose();
        }
        _skillSpriteLookup.Clear();
    }

    private sealed class SpriteDrawingData
    {
        private readonly int _originalSpriteWidth;
        private readonly int _originalSpriteHeight;

        private readonly PixelColourData _colourData;
        public IOrientation Orientation { get; }
        public DihedralTransformation DihedralTransformation { get; }
        public int ThisSpriteWidth { get; }
        public int ThisSpriteHeight { get; }

        public SpriteDrawingData(
            IOrientation orientation,
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
            DihedralTransformation.Transform(
                _originalSpriteWidth - 1,
                _originalSpriteHeight - 1,
                x0,
                y0,
                out var x1,
                out var y1);

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

    public Texture2D GetAnchorTexture()
    {
        return _textureLookup["anchor"];
    }

    public SkillSprite GetSkillSprite(Lemming lemming)
    {
        var key = $"{lemming.CurrentSkill.LemmingStateName}_{lemming.FacingDirection}_{lemming.Orientation}";

        return _skillSpriteLookup[key];
    }
}