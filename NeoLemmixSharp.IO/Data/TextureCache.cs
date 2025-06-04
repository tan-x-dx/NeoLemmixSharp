using Microsoft.Xna.Framework.Content;
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

    private static readonly Dictionary<TextureTypeKey, TextureUsageData> LongLivedTextures = new(LongLivedTextureCacheCapacity);
    private static readonly Dictionary<string, Texture2D> ShortLivedTextures = new(ShortLivedTextureCacheCapacity);

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
        const string SpritesFolder = "sprites/lemming/";

        for (var i = 0; i < LemmingActionConstants.NumberOfLemmingActions; i++)
        {
            var lemmingActionData = LemmingActionConstants.GetLemmingActionDataFromId(i);
            var pieceIdentifier = new PieceIdentifier(lemmingActionData.LemmingActionFileName);

            var spriteName = SpritesFolder + lemmingActionData.LemmingActionFileName;

            var sprite = contentManager.Load<Texture2D>(spriteName);
            var key = new TextureTypeKey(IoConstants.DefaultStyleIdentifier, pieceIdentifier, TextureType.LemmingSprite);

            LongLivedTextures.Add(key, new TextureUsageData(sprite));
        }
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
