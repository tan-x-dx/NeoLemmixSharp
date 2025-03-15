using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.CompilerServices;

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
        var result = new T[EngineConstants.NumberOfOrientations * EngineConstants.NumberOfFacingDirections];

        CreateSpritesForDirections(Orientation.Down, FacingDirection.Right);
        CreateSpritesForDirections(Orientation.Down, FacingDirection.Left);

        CreateSpritesForDirections(Orientation.Right, FacingDirection.Right);
        CreateSpritesForDirections(Orientation.Right, FacingDirection.Left);

        CreateSpritesForDirections(Orientation.Up, FacingDirection.Right);
        CreateSpritesForDirections(Orientation.Up, FacingDirection.Left);

        CreateSpritesForDirections(Orientation.Left, FacingDirection.Right);
        CreateSpritesForDirections(Orientation.Left, FacingDirection.Left);

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
        var pixels = new Color[texture.Width * texture.Height];
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
                        var pixel = pixelColorData[x0 + l0, y0 + f0];

                        spriteDrawingData.Set(pixel, x0, y0, l, f);
                    }
                }
            }
        }

        spriteDrawingData.DihedralTransformation.Transform(
            anchorPoint.X,
            anchorPoint.Y,
            spriteWidth - 1,
            spriteHeight - 1,
            out var anchorX1,
            out var anchorY1);

        var texture0 = spriteDrawingData.ToTexture(_graphicsDevice);
        var actionSprite = itemCreator(
            texture0,
            spriteDrawingData.ThisSpriteWidth,
            spriteDrawingData.ThisSpriteHeight,
            numberOfFrames,
            new LevelPosition(anchorX1, anchorY1));

        return actionSprite;
    }

    private readonly ref struct SpriteDrawingData
    {
        private readonly int _originalSpriteWidth;
        private readonly int _originalSpriteHeight;
        public readonly int ThisSpriteWidth;
        public readonly int ThisSpriteHeight;

        private readonly PixelColorData _colorData;
        public readonly DihedralTransformation DihedralTransformation;

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
            bool flipHorizontally;

            if ((rotNum & 1) != 0)
            {
                ThisSpriteWidth = _originalSpriteHeight;
                ThisSpriteHeight = _originalSpriteWidth;

                if (facingDirection == FacingDirection.Left)
                {
                    rotNum = (rotNum + 2) & 3;
                    flipHorizontally = true;
                }
                else
                {
                    flipHorizontally = false;
                }
            }
            else
            {
                ThisSpriteWidth = _originalSpriteWidth;
                ThisSpriteHeight = _originalSpriteHeight;

                flipHorizontally = facingDirection == FacingDirection.Left;
            }

            var uints = new Color[originalSpriteWidth * originalSpriteHeight * numberOfFrames * numberOfLayers];
            _colorData = new PixelColorData(ThisSpriteWidth * numberOfLayers, ThisSpriteHeight * numberOfFrames, uints);
            DihedralTransformation = new DihedralTransformation(rotNum, flipHorizontally);
        }

        public void Set(Color pixel, int x0, int y0, int layer, int frame)
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
            _colorData[x2, y2] = pixel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Texture2D ToTexture(GraphicsDevice graphicsDevice) => _colorData.CreateTexture(graphicsDevice);
    }
}
