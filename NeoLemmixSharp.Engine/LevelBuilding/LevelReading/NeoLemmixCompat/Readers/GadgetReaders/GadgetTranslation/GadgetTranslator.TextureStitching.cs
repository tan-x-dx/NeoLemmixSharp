using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
{
    private const int MaxStackAllocSize = 64;

    private SpriteData GetStitchedSpriteData(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var primaryTexture = LoadSourceGadgetTextures(archetypeData);

        var primaryTextureSpriteWidth = primaryTexture.Width;
        var primaryTextureSpriteHeight = primaryTexture.Height / archetypeData.PrimaryAnimationFrameCount;

        var spriteWidth = primaryTextureSpriteWidth;
        var spriteHeight = primaryTextureSpriteHeight;

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        if (animationDataSpan.IsEmpty)
        {
            return new SpriteData
            {
                Texture = primaryTexture,
                SpriteWidth = spriteWidth,
                SpriteHeight = spriteHeight,

                FrameCountsPerLayer = [archetypeData.PrimaryAnimationFrameCount]
            };
        }

        var maxNumberOfFrames = archetypeData.PrimaryAnimationFrameCount;
        foreach (var animationData in animationDataSpan)
        {
            var texture = animationData.Texture!;

            spriteWidth = Math.Max(spriteWidth, texture.Width);
            var animationSpriteHeight = texture.Height / animationData.NumberOfFrames;
            spriteHeight = Math.Max(spriteHeight, animationSpriteHeight);
            maxNumberOfFrames = Math.Max(maxNumberOfFrames, animationData.NumberOfFrames);
        }

        var numberOfLayers = 1 + animationDataSpan.Length;
        var result = new Texture2D(_graphicsDevice, spriteWidth * numberOfLayers, maxNumberOfFrames * spriteHeight);
        var resultTextureData = new Color[result.Width * result.Height];

        var frameData = new int[numberOfLayers];
        frameData[0] = archetypeData.PrimaryAnimationFrameCount;
        for (var i = 0; i < numberOfLayers - 1; i++)
        {
            frameData[i + 1] = animationDataSpan[i].NumberOfFrames;
        }

        var xOffset = 0;

        StitchTexture(
            primaryTexture,
            primaryTextureSpriteWidth,
            primaryTextureSpriteHeight,
            archetypeData.PrimaryAnimationFrameCount);

        xOffset += spriteWidth;

        foreach (var animationData in animationDataSpan)
        {
            var animationTexture = animationData.Texture!;

            var animationSpriteWidth = animationTexture.Width;
            var animationSpriteHeight = animationTexture.Height / animationData.NumberOfFrames;
            StitchTexture(
                animationTexture,
                animationSpriteWidth,
                animationSpriteHeight,
                animationData.NumberOfFrames);

            xOffset += spriteWidth;
        }

        result.SetData(resultTextureData);

        primaryTexture.Dispose();
        DisposeOfSourceTextures(archetypeData);

        /*
        // For debugging purposes
        var fileName = $@"C:\Temp\{archetypeData.Style}_{archetypeData.Gadget}.png";
        using (var fileStream = File.Create(fileName))
        {
            result.SaveAsPng(fileStream, result.Width, result.Height);
        }*/

        return new SpriteData
        {
            Texture = result,
            SpriteWidth = spriteWidth,
            SpriteHeight = spriteHeight,
            FrameCountsPerLayer = frameData
        };

        void StitchTexture(
            Texture2D sourceTexture,
            int currentSpriteWidth,
            int currentSpriteHeight,
            int numberOfFrames)
        {
            var sourceTextureData = new Color[sourceTexture.Width * sourceTexture.Height];
            sourceTexture.GetData(sourceTextureData);

            for (var i = 0; i < numberOfFrames; i++)
            {
                var sourceTextureWrapper = new SpanWrapper2D<Color>(sourceTextureData, sourceTexture.Width, sourceTexture.Height, 0, i * currentSpriteHeight, currentSpriteWidth, currentSpriteHeight);
                var resultTextureWrapper = new SpanWrapper2D<Color>(resultTextureData, result.Width, result.Height, xOffset, i * spriteHeight, currentSpriteWidth, currentSpriteHeight);

                for (var x = 0; x < sourceTextureWrapper.Width; x++)
                {
                    for (var y = 0; y < sourceTextureWrapper.Height; y++)
                    {
                        resultTextureWrapper[x, y] = sourceTextureWrapper[x, y];
                    }
                }
            }
        }
    }

    private Texture2D LoadSourceGadgetTextures(
        NeoLemmixGadgetArchetypeData archetypeData)
    {
        var styleObjectFolderPath = Path.Combine(
            RootDirectoryManager.RootDirectory,
            NeoLemmixFileExtensions.StyleFolderName,
            archetypeData.Style,
            NeoLemmixFileExtensions.GadgetFolderName);

        var gadgetName = archetypeData.GadgetPiece;

        var primaryTextureFileName = Path.Combine(
            styleObjectFolderPath,
            gadgetName);
        primaryTextureFileName = Path.ChangeExtension(primaryTextureFileName, ".png");

        var primaryTexture = Texture2D.FromFile(_graphicsDevice, primaryTextureFileName);

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        if (animationDataSpan.Length == 0)
            return primaryTexture;

        var bufferLength = 0;
        foreach (var animationData in animationDataSpan)
        {
            bufferLength = Math.Max(bufferLength, animationData.Name.Length);
        }

        bufferLength = bufferLength +
                       gadgetName.Length +
                       1 + // Add underscore
                       4; // Add file extension

        Span<char> fileNameSpan = bufferLength > MaxStackAllocSize
            ? new char[bufferLength]
            : stackalloc char[bufferLength];
        gadgetName.AsSpan().CopyTo(fileNameSpan);
        fileNameSpan[gadgetName.Length] = '_';

        foreach (var animationData in animationDataSpan)
        {
            var i = gadgetName.Length + 1;
            animationData.Name.AsSpan().CopyTo(fileNameSpan[i..]);
            i += animationData.Name.Length;
            // Instead of using Path.ChangeExtension,
            // just add the file extension here.
            fileNameSpan[i++] = '.';
            fileNameSpan[i++] = 'p';
            fileNameSpan[i++] = 'n';
            fileNameSpan[i++] = 'g';
            var animationFileName = fileNameSpan[..i].ToString();

            var filePath = Path.Combine(
                styleObjectFolderPath,
                animationFileName);
            var secondaryTexture = Texture2D.FromFile(_graphicsDevice, filePath);
            animationData.Texture = secondaryTexture;
        }

        return primaryTexture;
    }

    private static void DisposeOfSourceTextures(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);

        foreach (var animationData in animationDataSpan)
        {
            DisposableHelperMethods.DisposeOf(ref animationData.Texture);
        }
    }
}