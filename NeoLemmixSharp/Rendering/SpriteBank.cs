using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering;

public sealed class SpriteBank : IRenderable
{
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup;

    public TerrainSprite TerrainSprite { get; }
    public Texture2D BoxTexture { get; }
    public Texture2D AnchorTexture { get; }

    public SpriteBank(
        Dictionary<string, LemmingActionSpriteBundle> actionSpriteBundleLookup,
        TerrainSprite terrainSprite,
        Texture2D boxTexture,
        Texture2D anchorTexture)
    {
        _actionSpriteBundleLookup = actionSpriteBundleLookup;
        TerrainSprite = terrainSprite;
        BoxTexture = boxTexture;
        AnchorTexture = anchorTexture;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        TerrainSprite.Render(spriteBatch);
    }

    public void Dispose()
    {
        foreach (var actionSpriteBundle in _actionSpriteBundleLookup.Values)
        {
            actionSpriteBundle.Dispose();
        }

        _actionSpriteBundleLookup.Clear();

        TerrainSprite.Dispose();
        BoxTexture.Dispose();
        AnchorTexture.Dispose();
    }
}