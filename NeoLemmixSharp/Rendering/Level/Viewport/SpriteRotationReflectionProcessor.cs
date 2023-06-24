using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.LevelBuilding.Sprites;
using NeoLemmixSharp.Rendering.Level.Viewport.Lemming;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Rendering.Level.Viewport;

public sealed class SpriteRotationReflectionProcessor
{
    public delegate ActionSprite ActionSpriteCreator(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint);

    private readonly GraphicsDevice _graphicsDevice;

    public SpriteRotationReflectionProcessor(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public ActionSprite[] GenerateAllSpriteTypes(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ActionSpriteCreator actionSpriteCreator)
    {
        var result = new ActionSprite[8];

        var key = LemmingSpriteBank.GetKey(DownOrientation.Instance, RightFacingDirection.Instance);
        result[key] = actionSpriteCreator(texture, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint); // Don't need to process this texture, as it should be the correct version by default.
        key = LemmingSpriteBank.GetKey(DownOrientation.Instance, LeftFacingDirection.Instance);
        result[key] = Bar(texture, DownOrientation.Instance, LeftFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);

        key = LemmingSpriteBank.GetKey(LeftOrientation.Instance, RightFacingDirection.Instance);
        result[key] = Bar(texture, LeftOrientation.Instance, RightFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);
        key = LemmingSpriteBank.GetKey(LeftOrientation.Instance, LeftFacingDirection.Instance);
        result[key] = Bar(texture, LeftOrientation.Instance, LeftFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);

        key = LemmingSpriteBank.GetKey(RightOrientation.Instance, RightFacingDirection.Instance);
        result[key] = Bar(texture, RightOrientation.Instance, RightFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);
        key = LemmingSpriteBank.GetKey(RightOrientation.Instance, LeftFacingDirection.Instance);
        result[key] = Bar(texture, RightOrientation.Instance, LeftFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);

        key = LemmingSpriteBank.GetKey(UpOrientation.Instance, RightFacingDirection.Instance);
        result[key] = Bar(texture, UpOrientation.Instance, RightFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);
        key = LemmingSpriteBank.GetKey(UpOrientation.Instance, LeftFacingDirection.Instance);
        result[key] = Bar(texture, UpOrientation.Instance, LeftFacingDirection.Instance, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);

        return result;
    }

    private ActionSprite Bar(
        Texture2D texture,
        Orientation orientation,
        FacingDirection facingDirection,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ActionSpriteCreator actionSpriteCreator)
    {
        var pixels = new uint[texture.Width * texture.Height];
        texture.GetData(pixels);

        var pixelColourData = new PixelColourData(texture.Width, texture.Height, pixels);
        var spriteDrawingData = new SpriteDrawingData(orientation, facingDirection, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers);

        for (var l = 0; l < numberOfLayers; l++)
        {
            var l0 = l * spriteWidth;

            for (var f = 0; f < numberOfFrames; f++)
            {
                for (var x0 = 0; x0 < spriteWidth; x0++)
                {
                    for (var y0 = 0; y0 < spriteHeight; y0++)
                    {
                        var pixel = pixelColourData.Get(x0 + l0, y0 + f * spriteHeight);

                        spriteDrawingData.Set(pixel, x0, y0, l, f);
                    }
                }
            }
        }

        var texture0 = spriteDrawingData.ToTexture(_graphicsDevice);

        spriteDrawingData.DihedralTransformation.Transform(
            anchorPoint.X,
            anchorPoint.Y,
            spriteWidth - 1,
            spriteHeight - 1,
            out var footX1,
            out var footY1);

        var actionSprite = actionSpriteCreator(
            texture0,
            spriteDrawingData.ThisSpriteWidth,
            spriteDrawingData.ThisSpriteHeight,
            numberOfFrames,
            numberOfLayers,
            new LevelPosition(footX1, footY1));

        return actionSprite;
    }
    /*
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
    }*/
}
