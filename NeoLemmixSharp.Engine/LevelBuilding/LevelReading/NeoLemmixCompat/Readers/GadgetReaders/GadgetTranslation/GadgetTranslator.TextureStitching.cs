using Microsoft.Xna.Framework.Graphics;
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
        var primaryTexture = LoadNativeGadgetTextures(archetypeData);

        spriteWidth = primaryTexture.Width;
        spriteHeight = primaryTexture.Height / archetypeData.PrimaryAnimationFrameCount;

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
        if (animationDataSpan.IsEmpty)
            return primaryTexture;

        var maxTextureWidth = primaryTexture.Width;
        var maxTextureHeight = primaryTexture.Height;

        foreach (var animationData in animationDataSpan)
        {
            var texture = animationData.Texture!;

            maxTextureWidth += texture.Width;
            maxTextureHeight = Math.Max(maxTextureHeight, texture.Height);
        }

        var result = new Texture2D(_graphicsDevice, maxTextureWidth * (1 + animationDataSpan.Length), maxTextureHeight);
        var resultTextureData = new uint[result.Width * result.Height];

        var primaryTextureData = new uint[primaryTexture.Width * primaryTexture.Height];
        primaryTexture.GetData(primaryTextureData);

        var xOffset = StitchTexture(resultTextureData, result.Width, result.Height, 0, primaryTexture);

        foreach (var animationData in animationDataSpan)
        {
            var animationTexture = animationData.Texture!;
            var xOffsetDelta = StitchTexture(resultTextureData, result.Width, result.Height, xOffset, animationTexture);

            xOffset += xOffsetDelta;
        }

        result.SetData(resultTextureData);

        primaryTexture.Dispose();
        DisposeOfSourceTextures(archetypeData);

        return result;
    }

    private Texture2D LoadNativeGadgetTextures(
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
        int xOffset,
        Texture2D sourceTexture)
    {
        var animationTextureData = new uint[sourceTexture.Width * sourceTexture.Height];
        sourceTexture.GetData(animationTextureData);

        var animationTextureWrapper = new SpanWrapper2D(animationTextureData, sourceTexture.Width, sourceTexture.Height, 0, 0, sourceTexture.Width, sourceTexture.Height);
        var resultTextureWrapper = new SpanWrapper2D(resultTextureData, resultTextureWidth, resultTextureHeight, xOffset, 0, sourceTexture.Width, sourceTexture.Height);

        for (var x = 0; x < animationTextureWrapper.Width; x++)
        {
            for (var y = 0; y < animationTextureWrapper.Height; y++)
            {
                resultTextureWrapper[x, y] = animationTextureWrapper[x, y];
            }
        }

        return sourceTexture.Width;
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