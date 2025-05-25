using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data;

public static class TextureCache
{
    private const int ShortLivedTextureCacheCapacity = 8;
    private const int LongLivedTextureCacheCapacity = 64;

    private static GraphicsDevice GraphicsDevice { get; set; } = null!;

    private static readonly Dictionary<TextureTypeKey, TextureUsageData> LongLivedCachedTextures = new(LongLivedTextureCacheCapacity);
    private static readonly Dictionary<string, Texture2D> ShortLivedTextures = new(ShortLivedTextureCacheCapacity);

    public static void Initialise(GraphicsDevice graphicsDevice)
    {
        if (GraphicsDevice is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(TextureCache)} more than once!");

        GraphicsDevice = graphicsDevice;

        LoadDefaultTextures();
    }

    private static void LoadDefaultTextures()
    {
        var spriteDirectory = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            StyleCache.DefaultStyleIdentifier.ToString(),
            DefaultFileExtensions.LemmingsFolderName);

        for (var i = 0; i < EngineConstants.NumberOfLemmingActions; i++)
        {
            var lemmingActionData = Helpers.GetLemmingActionDataFromId(i);
            var pieceIdentifier = new PieceIdentifier(lemmingActionData.LemmingActionFileName);

            var spriteFilePath = Path.Combine(
                spriteDirectory,
                lemmingActionData.LemmingActionFileName);

            var pngPath = Path.ChangeExtension(spriteFilePath, "png");

            GetOrLoadTexture(
                pngPath,
                StyleCache.DefaultStyleIdentifier,
                pieceIdentifier,
                TextureType.LemmingSprite);
        }
    }

    public static Texture2D GetOrLoadTexture(
        string filePath,
        StyleIdentifier styleIdentifier,
        PieceIdentifier pieceIdentifier,
        TextureType textureType)
    {
        var key = new TextureTypeKey(styleIdentifier, pieceIdentifier, textureType);

        ref var textureUsageData = ref CollectionsMarshal.GetValueRefOrAddDefault(LongLivedCachedTextures, key, out var exists);

        var texture = exists
            ? textureUsageData.Texture
            : LoadTexture(filePath);

        textureUsageData = new TextureUsageData(texture, 0);
        return texture;
    }

    private static Texture2D LoadTexture(string filePath)
    {
        return Texture2D.FromFile(GraphicsDevice, filePath);
    }

    public static void CacheShortLivedTexture(Texture2D texture)
    {
        var textureName = texture.Name;
        if (string.IsNullOrWhiteSpace(textureName))
            throw new ArgumentException("Texture name not set!");

        if (!ShortLivedTextures.TryAdd(textureName, texture))
            throw new InvalidOperationException("Texture with that name is already present in the cache!");
    }

    public static void DisposeOfShortLivedTextures()
    {
        DisposableHelperMethods.DisposeOfAll(ShortLivedTextures);
    }

    private readonly struct TextureTypeKey(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier, TextureType textureType) : IEquatable<TextureTypeKey>
    {
        public readonly StyleIdentifier StyleIdentifier = styleIdentifier;
        public readonly PieceIdentifier PieceIdentifier = pieceIdentifier;
        public readonly TextureType TextureType = textureType;

        public bool Equals(TextureTypeKey other) => StyleIdentifier.Equals(other.StyleIdentifier) &&
                                                    PieceIdentifier.Equals(other.PieceIdentifier) &&
                                                    TextureType == other.TextureType;

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is TextureTypeKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(StyleIdentifier, PieceIdentifier, TextureType);
        public static bool operator ==(TextureTypeKey left, TextureTypeKey right) => left.Equals(right);
        public static bool operator !=(TextureTypeKey left, TextureTypeKey right) => !left.Equals(right);
    }

    private readonly struct TextureUsageData(Texture2D texture, int numberOfLevelsSinceLastUsed)
    {
        public readonly Texture2D Texture = texture;
        public readonly int NumberOfLevelsSinceLastUsed = numberOfLevelsSinceLastUsed;
    }

    public static void CleanUpOldTextures()
    {
        var notUsedTextureTypeKeys = new List<TextureTypeKey>(LongLivedTextureCacheCapacity);

        foreach (var kvp in LongLivedCachedTextures)
        {
            var numberOfLevelsSinceLastUsed = kvp.Value.NumberOfLevelsSinceLastUsed;

            numberOfLevelsSinceLastUsed++;

            if (numberOfLevelsSinceLastUsed > EngineConstants.NumberOfLevelsToKeepStyle)
            {
                notUsedTextureTypeKeys.Add(kvp.Key);
            }
        }

        foreach (var textureTypeKey in notUsedTextureTypeKeys)
        {
            if (StyleCache.DefaultStyleIdentifier.Equals(textureTypeKey.StyleIdentifier))
                continue;

            LongLivedCachedTextures[textureTypeKey].Texture.Dispose();
            LongLivedCachedTextures.Remove(textureTypeKey);
        }
    }
}
