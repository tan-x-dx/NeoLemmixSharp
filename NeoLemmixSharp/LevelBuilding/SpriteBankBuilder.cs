﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class SpriteBankBuilder
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup = new();

    public SpriteBankBuilder(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public SpriteBank BuildSpriteBank(
        ThemeData themeData,
        TerrainSprite terrainSprite)
    {
        var boxTexture = CreateBoxTexture();
        var anchorTexture = CreateAnchorTexture();

        LoadLemmingSprites(themeData);

        return new SpriteBank(
            _actionSpriteBundleLookup,
            terrainSprite,
            boxTexture,
            anchorTexture);
    }

    private void LoadLemmingSprites(ThemeData themeData)
    {
        foreach (var lemmingState in ILemmingAction.AllLemmingActions)
        {
            var pngFilePath = Path.Combine(themeData.LemmingSpritesFilePath, $"{lemmingState.LemmingActionName}.png");

            var spriteIdentifier = GetSpriteIdentifier(lemmingState.LemmingActionName);
            var spriteData = themeData.LemmingSpriteDataLookup[spriteIdentifier];

            var texture = Texture2D.FromFile(_graphicsDevice, pngFilePath);

            ProcessLemmingSpriteTexture(lemmingState.LemmingActionName, spriteData, texture);
        }
    }

    private static string GetSpriteIdentifier(string lemmingStateName)
    {
        return $"${lemmingStateName.ToUpperInvariant()}";
    }

    private Texture2D CreateAnchorTexture()
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
        return anchorTexture;
    }

    private Texture2D CreateBoxTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 1, 1);

        var white = Color.White;
        var x = new uint[1];
        for (var i = 0; i < x.Length; i++)
        {
            x[i] = white.PackedValue;
        }

        anchorTexture.SetData(x);
        return anchorTexture;
    }

    private void ProcessLemmingSpriteTexture(string stateName, LemmingSpriteData spriteData, Texture2D texture)
    {
        var originalPixelColourData = PixelColourData.GetPixelColourDataFromTexture(texture);

        var actionSpriteBundle = new LemmingActionSpriteBundle();
        ILemmingAction.LemmingActions[stateName].ActionSpriteBundle = actionSpriteBundle;

        _actionSpriteBundleLookup.Add(stateName, actionSpriteBundle);

        ProcessLefts(spriteData, originalPixelColourData, actionSpriteBundle);
        ProcessRights(spriteData, originalPixelColourData, actionSpriteBundle);
    }

    private void ProcessLefts(
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            spriteData,
            originalPixelColourData,
            0,
            spriteData.LeftFootX,
            spriteData.LeftFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetLeftActionSprite(b, a));
    }

    private void ProcessRights(
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            spriteData,
            originalPixelColourData,
            originalPixelColourData.Width / 2,
            spriteData.RightFootX,
            spriteData.RightFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetRightActionSprite(b, a));
    }

    private void CreateSprites(
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        int dx0,
        int footX,
        int footY,
        LemmingActionSpriteBundle actionSpriteBundle,
        Action<IOrientation, LemmingActionSpriteBundle, ActionSprite> setSprite)
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
            var texture = spriteDrawingData.ToTexture(_graphicsDevice);

            spriteDrawingData.DihedralTransformation.Transform(
                spriteWidth - 1,
                spriteHeight - 1,
                footX,
                footY,
                out var footX1,
                out var footY1);

            var actionSprite = new ActionSprite(
                texture,
                spriteDrawingData.ThisSpriteWidth,
                spriteDrawingData.ThisSpriteHeight,
                spriteData.NumberOfFrames,
                new LevelPosition(footX1, footY1));

            setSprite(spriteDrawingData.Orientation, actionSpriteBundle, actionSprite);
        }
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
}