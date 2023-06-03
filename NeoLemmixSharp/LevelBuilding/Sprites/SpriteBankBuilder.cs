using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding.Sprites;

public sealed class SpriteBankBuilder
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup = new();
    private readonly Dictionary<string, Texture2D> _textureLookup = new();

    public SpriteBankBuilder(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public SpriteBank BuildSpriteBank(
        ContentManager content,
        ThemeData themeData,
        TerrainSprite terrainSprite,
        ICollection<GadgetData> allGadgetData)
    {
        var anchorTexture = CreateAnchorTexture();
        var whitePixelTexture = CreateWhitePixelTexture();

        var cursorSprite = LoadCursorSprites(content);
        LoadLemmingSprites(themeData);
        LoadGadgetSprites(allGadgetData);
        LoadOtherTextures(content);

        return new SpriteBank(
            _actionSpriteBundleLookup,
            _textureLookup,
            terrainSprite)
        {
            AnchorTexture = anchorTexture,
            WhitePixelTexture = whitePixelTexture,
            LevelCursorSprite = cursorSprite
        };
    }

    private static LevelCursorSprite LoadCursorSprites(ContentManager content)
    {
        var standardCursorTexture = content.Load<Texture2D>("cursor/standard");
        var focusedCursorTexture = content.Load<Texture2D>("cursor/focused");

        return new LevelCursorSprite(standardCursorTexture, focusedCursorTexture);
    }

    private void LoadLemmingSprites(ThemeData themeData)
    {
        foreach (var lemmingState in LemmingAction.AllLemmingActions.Where(la => la.ActionId >= 0))
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

    private Texture2D CreateBoxTexture()
    {
        var boxTexture = new Texture2D(_graphicsDevice, 1, 1);

        var white = Color.White;
        var x = new uint[1];
        for (var i = 0; i < x.Length; i++)
        {
            x[i] = white.PackedValue;
        }

        boxTexture.SetData(x);
        return boxTexture;
    }

    private Texture2D CreateAnchorTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 3, 3);

        var red = new Color(200, 0, 0, 255).PackedValue;
        var yellow = new Color(200, 200, 0, 255).PackedValue;

        var x = new uint[9];
        x[1] = red;
        x[3] = red;
        x[4] = yellow;
        x[5] = red;
        x[7] = red;
        anchorTexture.SetData(x);
        return anchorTexture;
    }

    private Texture2D CreateWhitePixelTexture()
    {
        var blackPixelTexture = new Texture2D(_graphicsDevice, 1, 1);

        var x = new[] { Color.White.PackedValue };

        blackPixelTexture.SetData(x);
        return blackPixelTexture;
    }

    private void ProcessLemmingSpriteTexture(string stateName, LemmingSpriteData spriteData, Texture2D texture)
    {
        var originalPixelColourData = PixelColourData.GetPixelColourDataFromTexture(texture);

        var actionSpriteBundle = new LemmingActionSpriteBundle();
        LemmingAction.LemmingActions[stateName].ActionSpriteBundle = actionSpriteBundle;

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
        Action<Orientation, LemmingActionSpriteBundle, ActionSprite> setSprite)
    {
        var spriteWidth = originalPixelColourData.Width / 2;
        var spriteHeight = originalPixelColourData.Height / spriteData.NumberOfFrames;

        var spriteDrawingDatas = Orientation
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

            spriteDrawingData.DihedralTransformation.Transform(footX,
                footY,
                spriteWidth - 1,
                spriteHeight - 1, out var footX1, out var footY1);

            var actionSprite = new ActionSprite(
                texture,
                spriteDrawingData.ThisSpriteWidth,
                spriteDrawingData.ThisSpriteHeight,
                spriteData.NumberOfFrames,
                new LevelPosition(footX1, footY1));

            setSprite(spriteDrawingData.Orientation, actionSpriteBundle, actionSprite);
        }
    }

    private void LoadGadgetSprites(ICollection<GadgetData> allGadgetData)
    {
    }

    private void LoadOtherTextures(ContentManager contentManager)
    {
        RegisterTexture("panel/empty_slot");
        RegisterTexture("panel/icon_cpm_and_replay");
        RegisterTexture("panel/icon_directional");
        RegisterTexture("panel/icon_ff");
        RegisterTexture("panel/icon_frameskip");
        RegisterTexture("panel/icon_nuke");
        RegisterTexture("panel/icon_pause");
        RegisterTexture("panel/icon_restart");
        RegisterTexture("panel/icon_rr_minus");
        RegisterTexture("panel/icon_rr_plus");
        RegisterTexture("panel/minimap_region");
        RegisterTexture("panel/panel_icons");
        RegisterTexture("panel/skill_count_erase");
        RegisterTexture("panel/skill_panels");
        RegisterTexture("panel/skill_selected");

        void RegisterTexture(string textureName)
        {
            var texture = contentManager.Load<Texture2D>(textureName);
            _textureLookup.Add(textureName, texture);
        }
    }
}