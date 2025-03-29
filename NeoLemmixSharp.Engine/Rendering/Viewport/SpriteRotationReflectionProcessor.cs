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
        LevelSize spriteSize,
        int numberOfFrames,
        LevelPosition anchorPoint);

    private readonly GraphicsDevice _graphicsDevice;

    public SpriteRotationReflectionProcessor(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public T[] CreateAllSpriteTypes(
        Texture2D texture,
        LevelSize spriteSize,
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

            result[key] = CreateSpriteType(texture, orientation, facingDirection, spriteSize, numberOfFrames, numberOfLayers, anchorPoint, itemCreator);
        }
    }

    public T CreateSpriteType(
        Texture2D texture,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelSize spriteSize,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ItemCreator itemCreator)
    {
        var pixels = new Color[texture.Width * texture.Height];
        texture.GetData(pixels);

        var pixelColorData = new PixelColorData(texture.Width, texture.Height, pixels);
        var spriteDrawingData = new SpriteDrawingData(orientation, facingDirection, spriteSize, numberOfFrames, numberOfLayers);

        
        for (var l = 0; l < numberOfLayers; l++)
        {
            var l0 = l * spriteSize.W;

            for (var f = 0; f < numberOfFrames; f++)
            {
                var f0 = f * spriteSize.H;

                for (var x0 = 0; x0 < spriteSize.W; x0++)
                {
                    for (var y0 = 0; y0 < spriteSize.H; y0++)
                    {
                        var pixel = pixelColorData[x0 + l0, y0 + f0];
                        var p0 = new LevelPosition(x0, y0);

                        spriteDrawingData.Set(pixel, p0, l, f);
                    }
                }
            }
        }

        var p1 = spriteDrawingData.DihedralTransformation.Transform(
            anchorPoint,
            spriteSize);

        var texture0 = spriteDrawingData.ToTexture(_graphicsDevice);
        var actionSprite = itemCreator(
            texture0,
            spriteDrawingData.ThisSpriteSize,
            numberOfFrames,
            p1);

        return actionSprite;
    }

    private readonly ref struct SpriteDrawingData
    {
        public readonly DihedralTransformation DihedralTransformation;
        private readonly LevelSize _originalSpriteSize;
        public readonly LevelSize ThisSpriteSize;

        private readonly PixelColorData _colorData;

        public SpriteDrawingData(
            Orientation orientation,
            FacingDirection facingDirection,
            LevelSize originalSpriteSize,
            int numberOfFrames,
            int numberOfLayers)
        {
            DihedralTransformation = new DihedralTransformation(orientation, facingDirection);
            _originalSpriteSize = originalSpriteSize;
            ThisSpriteSize = DihedralTransformation.Transform(originalSpriteSize);

            var uints = new Color[originalSpriteSize.Area() * numberOfFrames * numberOfLayers];
            _colorData = new PixelColorData(ThisSpriteSize.W * numberOfLayers, ThisSpriteSize.H * numberOfFrames, uints);
        }

        public void Set(Color pixel, LevelPosition p0, int layer, int frame)
        {
            var p1 = DihedralTransformation.Transform(
                p0,
                _originalSpriteSize);
            var offset = new LevelPosition(ThisSpriteSize.W * layer, ThisSpriteSize.H * frame);

            p1 += offset;
            _colorData[p1] = pixel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Texture2D ToTexture(GraphicsDevice graphicsDevice) => _colorData.CreateTexture(graphicsDevice);
    }
}
