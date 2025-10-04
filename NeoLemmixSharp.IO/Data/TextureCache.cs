using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data;

public static class TextureCache
{
    private const int LevelSpecificTextureCacheCapacity = 8;
    private const int LongLivedTextureCacheCapacity = 64;

    private static GraphicsDevice GraphicsDevice { get; set; } = null!;

    private static readonly Dictionary<TextureTypeKey, TextureUsageData> LongLivedTextures = new(LongLivedTextureCacheCapacity);
    private static readonly Dictionary<string, Texture2D> LevelSpecificTextures = new(LevelSpecificTextureCacheCapacity);

    public static void Initialise(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        if (GraphicsDevice is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(TextureCache)} more than once!");

        GraphicsDevice = graphicsDevice;

        LoadDefaultTextures(contentManager);
    }

    private static void LoadDefaultTextures(ContentManager contentManager)
    {
        for (var i = 0; i < LemmingActionConstants.NumberOfLemmingActions; i++)
        {
            var lemmingActionData = LemmingActionConstants.GetLemmingActionDataFromId(i);
            LoadLemmingActionTexture(lemmingActionData, contentManager);
        }
    }

    private static void LoadLemmingActionTexture(LemmingActionConstants.LemmingActionLookupData lemmingActionData, ContentManager contentManager)
    {
        const string SpritesFolder = "sprites/lemming/";

        var pieceIdentifier = new PieceIdentifier(lemmingActionData.LemmingActionFileName);

        var spriteName = SpritesFolder + lemmingActionData.LemmingActionFileName;

        var sprite = contentManager.Load<Texture2D>(spriteName);
        var key = new TextureTypeKey(IoConstants.DefaultStyleIdentifier, pieceIdentifier, TextureType.LemmingSprite);

        LongLivedTextures.Add(key, new TextureUsageData(sprite));
    }

    public static Texture2D GetOrLoadTexture(
        string filePath,
        StyleIdentifier styleIdentifier,
        PieceIdentifier pieceIdentifier,
        TextureType textureType)
    {
        var key = new TextureTypeKey(styleIdentifier, pieceIdentifier, textureType);

        ref var textureUsageData = ref CollectionsMarshal.GetValueRefOrAddDefault(LongLivedTextures, key, out var exists);

        var texture = exists
            ? textureUsageData.Texture
            : LoadTexture(filePath);

        textureUsageData = new TextureUsageData(texture);

        return texture;
    }

    private static Texture2D LoadTexture(string filePath)
    {
        return Texture2D.FromFile(GraphicsDevice, filePath);
    }

    public static void CacheLevelSpecificTexture(Texture2D texture)
    {
        var textureName = texture.Name;
        if (string.IsNullOrWhiteSpace(textureName))
            throw new ArgumentException("Texture name not set!");

        if (!LevelSpecificTextures.TryAdd(textureName, texture))
            throw new InvalidOperationException("Texture with that name is already present in the cache!");
    }

    public static void DisposeOfLevelSpecificTextures()
    {
        DisposableHelperMethods.DisposeOfAll(LevelSpecificTextures);
    }

    public static void CleanUpOldTextures()
    {
        var notUsedTextureTypeKeys = new ArrayListWrapper<TextureTypeKey>(LongLivedTextures.Count);

        foreach (var textureTypeKey in LongLivedTextures.Keys)
        {
            if (IoConstants.DefaultStyleIdentifier.Equals(textureTypeKey.StyleIdentifier))
                continue;

            ref var usageData = ref CollectionsMarshal.GetValueRefOrNullRef(LongLivedTextures, textureTypeKey);
            usageData = usageData.IncrementTimeSinceLastUsage();

            if (usageData.NumberOfLevelsSinceLastUsed > IoConstants.NumberOfLevelsToKeepStyle)
            {
                notUsedTextureTypeKeys.Add(textureTypeKey);
            }
        }

        foreach (var textureTypeKey in notUsedTextureTypeKeys.AsReadOnlySpan())
        {
            if (IoConstants.DefaultStyleIdentifier.Equals(textureTypeKey.StyleIdentifier))
                continue;

            LongLivedTextures[textureTypeKey].Dispose();
            LongLivedTextures.Remove(textureTypeKey);
        }
    }

    private readonly record struct TextureTypeKey(StyleIdentifier StyleIdentifier, PieceIdentifier PieceIdentifier, TextureType TextureType);

    private readonly struct TextureUsageData : IDisposable
    {
        public readonly Texture2D Texture;
        public readonly int NumberOfLevelsSinceLastUsed;

        public TextureUsageData(Texture2D texture)
        {
            Texture = texture;
        }

        private TextureUsageData(Texture2D texture, int numberOfLevelsSinceLastUsed)
        {
            Texture = texture;
            NumberOfLevelsSinceLastUsed = numberOfLevelsSinceLastUsed;
        }

        public void Dispose() => Texture.Dispose();

        public TextureUsageData IncrementTimeSinceLastUsage() => new(Texture, NumberOfLevelsSinceLastUsed + 1);
    }
}
