using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using NeoLemmixSharp.LevelBuilding.Sprites;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Rendering2.Level.ViewportSprites;

public sealed class SpriteRotationReflectionProcessor
{
    private readonly GraphicsDevice _graphicsDevice;

    public ILevelObjectRenderer RightLeftSprite { get; private set; }
    public ILevelObjectRenderer RightRightSprite { get; private set; }

    public ILevelObjectRenderer UpLeftSprite { get; private set; }
    public ILevelObjectRenderer UpRightSprite { get; private set; }

    public ILevelObjectRenderer LeftLeftSprite { get; private set; }
    public ILevelObjectRenderer LeftRightSprite { get; private set; }

    public ILevelObjectRenderer DownLeftSprite { get; private set; }
    public ILevelObjectRenderer DownRightSprite { get; private set; }

    public SpriteRotationReflectionProcessor(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void Foo()
    {

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
        ISpriteData spriteData,
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

            spriteDrawingData.DihedralTransformation.Transform(
                footX,
                footY,
                spriteWidth - 1,
                spriteHeight - 1,
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
}

public interface ISpriteData
{
    int NumberOfFrames { get; }
}