using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using System.Runtime.CompilerServices;
using NeoLemmixSharp.Engine.LevelBuilding.Sprites;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class SpriteRotationReflectionProcessor<T>
{
    public delegate T ItemCreator(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint);

    private readonly GraphicsDevice _graphicsDevice;

    public SpriteRotationReflectionProcessor(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public T[] CreateAllSpriteTypes(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ItemCreator itemCreator)
    {
        var result = new T[8];

        CreateSpritesForDirections(DownOrientation.Instance, FacingDirection.RightInstance);
        CreateSpritesForDirections(DownOrientation.Instance, FacingDirection.LeftInstance);

        CreateSpritesForDirections(RightOrientation.Instance, FacingDirection.RightInstance);
        CreateSpritesForDirections(RightOrientation.Instance, FacingDirection.LeftInstance);

        CreateSpritesForDirections(UpOrientation.Instance, FacingDirection.RightInstance);
        CreateSpritesForDirections(UpOrientation.Instance, FacingDirection.LeftInstance);

        CreateSpritesForDirections(LeftOrientation.Instance, FacingDirection.RightInstance);
        CreateSpritesForDirections(LeftOrientation.Instance, FacingDirection.LeftInstance);

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CreateSpritesForDirections(Orientation orientation, FacingDirection facingDirection)
        {
            var key = LemmingSpriteBank.GetKey(orientation, facingDirection);

            result[key] = CreateSpriteType(texture, orientation, facingDirection, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint, itemCreator);
        }
    }

    public T CreateSpriteType(
        Texture2D texture,
        Orientation orientation,
        FacingDirection facingDirection,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ItemCreator itemCreator)
    {
        var pixels = new uint[texture.Width * texture.Height];
        texture.GetData(pixels);

        var pixelColorData = new PixelColorData(texture.Width, texture.Height, pixels);
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
                        var pixel = pixelColorData.Get(x0 + l0, y0 + f0);

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

        var actionSprite = itemCreator(
            texture0,
            spriteDrawingData.ThisSpriteWidth,
            spriteDrawingData.ThisSpriteHeight,
            numberOfFrames,
            new LevelPosition(footX1, footY1));

        return actionSprite;
    }
}
