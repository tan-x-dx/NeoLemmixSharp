using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LemmingSpriteBankBuilder
{
    public static LemmingSpriteBank BuildLemmingSpriteBank(LevelData levelData)
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

    private static SpriteBankData CreateSpriteBankData(ThemeData themeData)
    {
        var lemmingSpriteData = themeData.LemmingSpriteData;

        var lemmingActionSprites = new LemmingActionSprite[LemmingActionConstants.NumberOfLemmingActions];

        var spriteDirectory = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            lemmingSpriteData.LemmingSpriteStyleIdentifier.ToString(),
            DefaultFileExtensions.LemmingsFolderName);

        for (var i = 0; i < LemmingActionConstants.NumberOfLemmingActions; i++)
        {
            lemmingActionSprites[i] = CreateLemmingActionSprite(
                lemmingSpriteData,
                spriteDirectory,
                lemmingSpriteData.LemmingActionSpriteData[i]);
        }

        return new SpriteBankData(
            lemmingActionSprites,
            lemmingSpriteData.TribeColorData.ToArray());
    }

    private static LemmingActionSprite CreateLemmingActionSprite(
        LemmingSpriteData lemmingSpriteData,
        string spriteDirectory,
        LemmingActionSpriteData lemmingActionSpriteData)
    {
        var lemmingActionData = LemmingActionConstants.GetLemmingActionDataFromId(lemmingActionSpriteData.LemmingActionId);

        var spriteFilePath = Path.Combine(
            spriteDirectory,
            lemmingActionData.LemmingActionFileName);

        var pngPath = RootDirectoryManager.GetCorrespondingImageFile(spriteFilePath);

        var spriteTexture = TextureCache.GetOrLoadTexture(
            pngPath,
            lemmingSpriteData.LemmingSpriteStyleIdentifier,
            new PieceIdentifier(lemmingActionData.LemmingActionFileName),
            TextureType.LemmingSprite);

        var lemmingAnimationLayers = lemmingActionSpriteData.Layers;
        var spriteSize = SpriteHelpers.DetermineSpriteSize(
            lemmingActionData.LemmingActionFileName,
            spriteTexture,
            lemmingAnimationLayers.Length,
            lemmingActionData.NumberOfAnimationFrames);

        var layers = new LemmingActionLayerRenderer[lemmingAnimationLayers.Length];

        for (var i = 0; i < layers.Length; i++)
        {
            layers[i] = CreateLemmingActionLayerRenderer(
                spriteTexture,
                spriteSize,
                lemmingAnimationLayers[i]);
        }

        return new LemmingActionSprite(
            spriteTexture,
            lemmingActionSpriteData.AnchorPoint,
            spriteSize,
            layers);
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
