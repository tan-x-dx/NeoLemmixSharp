using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Rendering.Level.Viewport.Lemming;
using NeoLemmixSharp.Io.LevelReading.Sprites;

namespace NeoLemmixSharp.Engine.Rendering.Level.Viewport;

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

    public ActionSprite[] CreateAllSpriteTypes(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ActionSpriteCreator actionSpriteCreator)
    {
        var result = new ActionSprite[8];

        CreateSpritesForDirections(DownOrientation.Instance, RightFacingDirection.Instance);
        CreateSpritesForDirections(DownOrientation.Instance, LeftFacingDirection.Instance);

        CreateSpritesForDirections(RightOrientation.Instance, RightFacingDirection.Instance);
        CreateSpritesForDirections(RightOrientation.Instance, LeftFacingDirection.Instance);

        CreateSpritesForDirections(UpOrientation.Instance, RightFacingDirection.Instance);
        CreateSpritesForDirections(UpOrientation.Instance, LeftFacingDirection.Instance);

        CreateSpritesForDirections(LeftOrientation.Instance, RightFacingDirection.Instance);
        CreateSpritesForDirections(LeftOrientation.Instance, LeftFacingDirection.Instance);

        return result;

        void CreateSpritesForDirections(Orientation orientation, FacingDirection facingDirection)
        {
            var key = LemmingSpriteBank.GetKey(orientation, facingDirection);

            result[key] = CreateSpriteType(texture, orientation, facingDirection, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, actionSpriteCreator);
        }
    }

    private ActionSprite CreateSpriteType(
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
                var f0 = f * spriteHeight;

                for (var x0 = 0; x0 < spriteWidth; x0++)
                {
                    for (var y0 = 0; y0 < spriteHeight; y0++)
                    {
                        var pixel = pixelColourData.Get(x0 + l0, y0 + f0);

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
}
