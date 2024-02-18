﻿using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public sealed partial class GadgetTranslator
{
    private const int MaxStackAllocSize = 64;

    private Texture2D GetStitchedTextures(
        NeoLemmixGadgetArchetypeData archetypeData,
        out int spriteWidth,
        out int spriteHeight)
    {
        var primaryTexture = LoadSourceGadgetTextures(archetypeData);

        var maxTextureHeight = primaryTexture.Height;

        var primaryTextureSpriteWidth = primaryTexture.Width;
        var primaryTextureSpriteHeight = primaryTexture.Height / archetypeData.PrimaryAnimationFrameCount;

        spriteWidth = primaryTextureSpriteWidth;
        spriteHeight = primaryTextureSpriteHeight;

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        if (animationDataSpan.IsEmpty)
            return primaryTexture;

        foreach (var animationData in animationDataSpan)
        {
            var texture = animationData.Texture!;

            maxTextureHeight = Math.Max(maxTextureHeight, texture.Height);

            spriteWidth = Math.Max(spriteWidth, texture.Width);
            var animationSpriteHeight = texture.Height / animationData.NumberOfFrames;
            spriteHeight = Math.Max(spriteHeight, animationSpriteHeight);
        }

        var result = new Texture2D(_graphicsDevice, spriteWidth * (1 + animationDataSpan.Length), maxTextureHeight);
        var resultTextureData = new uint[result.Width * result.Height];

        var xOffset = StitchTexture(
            resultTextureData,
            result.Width,
            result.Height,
            spriteWidth,
            spriteHeight,
            0,
            primaryTexture,
            primaryTextureSpriteWidth,
            primaryTextureSpriteHeight,
            archetypeData.PrimaryAnimationFrameCount);

        foreach (var animationData in animationDataSpan)
        {
            var animationTexture = animationData.Texture!;

            var animationSpriteWidth = animationTexture.Width;
            var animationSpriteHeight = animationTexture.Height / animationData.NumberOfFrames;
            var xOffsetDelta = StitchTexture(
                resultTextureData,
                result.Width,
                result.Height,
                spriteWidth,
                spriteHeight,
                xOffset,
                animationTexture,
                animationSpriteWidth,
                animationSpriteHeight,
                animationData.NumberOfFrames);

            xOffset += xOffsetDelta;
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

        return result;
    }

    private Texture2D LoadSourceGadgetTextures(
        NeoLemmixGadgetArchetypeData archetypeData)
    {
        var styleObjectFolderPath = Path.Combine(
            RootDirectoryManager.RootDirectory,
            NeoLemmixFileExtensions.StyleFolderName,
            archetypeData.Style!,
            NeoLemmixFileExtensions.GadgetFolderName);

        var gadgetName = archetypeData.Gadget!;

        var primaryTextureFileName = Path.Combine(
            styleObjectFolderPath,
            gadgetName);
        primaryTextureFileName = Path.ChangeExtension(primaryTextureFileName, ".png");

        var primaryTexture = Texture2D.FromFile(_graphicsDevice, primaryTextureFileName);

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        if (animationDataSpan.Length == 0)
            return primaryTexture;

        var bufferLength = int.MinValue;
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

    private static int StitchTexture(
        uint[] resultTextureData,
        int resultTextureWidth,
        int resultTextureHeight,
        int resultSpriteWidth,
        int resultSpriteHeight,
        int xOffset,
        Texture2D sourceTexture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames)
    {
        var sourceTextureData = new uint[sourceTexture.Width * sourceTexture.Height];
        sourceTexture.GetData(sourceTextureData);

        for (var i = 0; i < numberOfFrames; i++)
        {
            var yOffset = i * spriteHeight;

            var sourceTextureWrapper = new SpanWrapper2D(sourceTextureData, sourceTexture.Width, sourceTexture.Height, 0, yOffset, spriteWidth, spriteHeight);
            var resultTextureWrapper = new SpanWrapper2D(resultTextureData, resultTextureWidth, resultTextureHeight, xOffset, i * resultSpriteHeight, spriteWidth, spriteHeight);

            for (var x = 0; x < sourceTextureWrapper.Width; x++)
            {
                for (var y = 0; y < sourceTextureWrapper.Height; y++)
                {
                    resultTextureWrapper[x, y] = sourceTextureWrapper[x, y];
                }
            }
        }

        return resultSpriteWidth;
    }

    private static void DisposeOfSourceTextures(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        foreach (var animationData in animationDataSpan)
        {
            animationData.Texture!.Dispose();
            animationData.Texture = null;
        }
    }
}