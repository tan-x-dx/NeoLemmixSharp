using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class SpriteHelpers
{
    public static Size DetermineSpriteSize(
        string spriteFileName,
        Texture2D spriteTexture,
        int numberOfLayers,
        int numberOfAnimationFrames)
    {
        var (widthQuotient, widthRemainder) = int.DivRem(spriteTexture.Width, numberOfLayers);
        var (heightQuotient, heightRemainder) = int.DivRem(spriteTexture.Height, numberOfAnimationFrames);

        if (widthRemainder != 0)
            ThrowInvalidDimensionsException(spriteFileName, "WIDTH", spriteTexture.Width, numberOfLayers);

        if (heightRemainder != 0)
            ThrowInvalidDimensionsException(spriteFileName, "HEIGHT", spriteTexture.Height, numberOfAnimationFrames);

        return new Size(widthQuotient, heightQuotient);
    }

    [DoesNotReturn]
    private static void ThrowInvalidDimensionsException(
        string spriteFileName,
        string widthOrHeight,
        int texturelength,
        int numberToDivideBy)
    {
        throw new InvalidOperationException(
            $"Error loading sprite data! File: {spriteFileName}. " +
            $"File {widthOrHeight} must be divisible by {numberToDivideBy}, actually is {texturelength}");
    }
}
