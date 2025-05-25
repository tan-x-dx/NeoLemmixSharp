using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly ref struct LemmingSpriteBankBuilder
{
    public LemmingSpriteBank BuildLemmingSpriteBank(LevelData levelData)
    {
        var listLookup = new ListLookup<StyleIdentifier, SpriteBankData>(EngineConstants.MaxNumberOfTribes);

        foreach (var tribeIdentifier in levelData.TribeIdentifiers)
        {
            var styleFormatPair = new StyleFormatPair(
                tribeIdentifier.StyleIdentifier,
                levelData.FileFormatType);
            var themeData = StyleCache.GetThemeData(styleFormatPair);

            if (!listLookup.ContainsKey(tribeIdentifier.StyleIdentifier))
            {
                listLookup.Add(tribeIdentifier.StyleIdentifier, CreateSpriteBankData(themeData));
            }
        }

        return new LemmingSpriteBank(listLookup);
    }

    private SpriteBankData CreateSpriteBankData(
        ThemeData themeData)
    {
        var lemmingActionSprites = new LemmingActionSprite[EngineConstants.NumberOfLemmingActions];

        var spriteDirectory = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            themeData.StyleIdentifier.ToString(),
            DefaultFileExtensions.LemmingsFolderName);

        for (var i = 0; i < EngineConstants.NumberOfLemmingActions; i++)
        {
            lemmingActionSprites[i] = CreateLemmingActionSprite(
                themeData,
                spriteDirectory,
                themeData.LemmingActionSpriteData[i]);
        }

        ReadOnlySpan<TribeColorData> tribeColorDataSpan = themeData.TribeColorData;

        return new SpriteBankData(
            lemmingActionSprites,
            tribeColorDataSpan.ToArray());
    }

    private LemmingActionSprite CreateLemmingActionSprite(
        ThemeData themeData,
        string spriteDirectory,
        LemmingActionSpriteData lemmingActionSpriteData)
    {
        var lemmingActionData = LemmingActionHelpers.GetLemmingActionDataFromId(lemmingActionSpriteData.LemmingActionId);

        var spriteFilePath = Path.Combine(
            spriteDirectory,
            lemmingActionData.LemmingActionFileName);

        var pngPath = Path.ChangeExtension(spriteFilePath, "png");

        var spriteTexture = TextureCache.GetOrLoadTexture(
            pngPath,
            themeData.StyleIdentifier,
            new PieceIdentifier(lemmingActionData.LemmingActionFileName),
            TextureType.LemmingSprite);

        var spriteSize = DetermineSpriteSize(
            lemmingActionData.LemmingActionFileName,
            spriteTexture,
            lemmingActionSpriteData.Layers.Length,
            lemmingActionData.NumberOfAnimationFrames);

        var layers = new LemmingActionLayerRenderer[lemmingActionSpriteData.Layers.Length];

        for (var i = 0; i < layers.Length; i++)
        {
            layers[i] = CreateLemmingActionLayerRenderer(
                spriteTexture,
                spriteSize,
                lemmingActionSpriteData.Layers[i]);
        }

        return new LemmingActionSprite(
            spriteTexture,
            lemmingActionSpriteData.AnchorPoint,
            spriteSize,
            layers);
    }

    private static Size DetermineSpriteSize(
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
            $"Error loading sprite data! File:{spriteFileName}. " +
            $"File {widthOrHeight} must be a multiple of {numberToDivideBy}, actually was {texturelength}");
    }

    private static LemmingActionLayerRenderer CreateLemmingActionLayerRenderer(
        Texture2D spriteTexture,
        Size spriteSize,
        LemmingActionSpriteLayerData lemmingActionSpriteLayerData)
    {
        return new LemmingActionLayerRenderer(
            spriteTexture,
            spriteSize.W * lemmingActionSpriteLayerData.Layer,
            lemmingActionSpriteLayerData.ColorType);
    }
}
