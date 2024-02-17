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

        var maxSpriteWidth = primaryTexture.Width;
        var maxSpriteHeight = primaryTexture.Height;

        foreach (var animationData in animationDataSpan)
        {
            maxSpriteWidth += animationData.Texture!.Width;
            maxSpriteHeight = Math.Max(maxSpriteHeight, animationData.Texture.Height);
        }

        var result = new Texture2D(_graphicsDevice, maxSpriteWidth, maxSpriteHeight);

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

        foreach (var animationData in archetypeData.AnimationData)
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
}