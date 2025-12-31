using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Data;

public static class UiSprites
{
    internal static Texture2D BevelTexture { get; private set; } = null!;
    public static SpriteFont UiFont { get; private set; } = null!;

    public static void Initialise(ContentManager contentManager)
    {
        if (BevelTexture is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(UiSprites)} more than once!");

        UiFont = contentManager.Load<SpriteFont>("Fonts/UiFont");

        BevelTexture = contentManager.Load<Texture2D>("menu/bevel");
    }

    public static void DrawBeveledRectangle(
        SpriteBatch spriteBatch,
        Component c)
    {
        var dest = new Rectangle(
            c.Left,
            c.Top,
            c.Width,
            c.Height);

        var colors = c.Colors.AsSpan();
        var color = colors.At((int)c.State);

        DrawNineSlicedBeveledRectangle(spriteBatch, dest, color);
    }

    private static void DrawNineSlicedBeveledRectangle(
        SpriteBatch spriteBatch,
        Rectangle dest,
        Color color)
    {
        const int SourceSize = 64;
        const int ChunkSize = 4;

        var source = new Rectangle(0, 0, ChunkSize, ChunkSize);
        var subDest = new Rectangle(dest.X, dest.Y, ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(ChunkSize, 0, SourceSize - 2 * ChunkSize, ChunkSize);
        subDest = new Rectangle(dest.X + ChunkSize, dest.Y, dest.Width - 2 * ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(SourceSize - ChunkSize, 0, ChunkSize, ChunkSize);
        subDest = new Rectangle(dest.Right - ChunkSize, dest.Y, ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);


        source = new Rectangle(0, ChunkSize, ChunkSize, SourceSize - 2 * ChunkSize);
        subDest = new Rectangle(dest.X, dest.Y + ChunkSize, ChunkSize, dest.Height - 2 * ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(ChunkSize, ChunkSize, SourceSize - 2 * ChunkSize, SourceSize - 2 * ChunkSize);
        subDest = new Rectangle(dest.X + ChunkSize, dest.Y + ChunkSize, dest.Width - 2 * ChunkSize, dest.Height - 2 * ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(SourceSize - ChunkSize, ChunkSize, ChunkSize, SourceSize - 2 * ChunkSize);
        subDest = new Rectangle(dest.Right - ChunkSize, dest.Y + ChunkSize, ChunkSize, dest.Height - 2 * ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);


        source = new Rectangle(0, SourceSize - ChunkSize, ChunkSize, ChunkSize);
        subDest = new Rectangle(dest.X, dest.Bottom - ChunkSize, ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(ChunkSize, SourceSize - ChunkSize, SourceSize - 2 * ChunkSize, ChunkSize);
        subDest = new Rectangle(dest.X + ChunkSize, dest.Bottom - ChunkSize, dest.Width - 2 * ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(SourceSize - ChunkSize, SourceSize - ChunkSize, ChunkSize, ChunkSize);
        subDest = new Rectangle(dest.Right - ChunkSize, dest.Bottom - ChunkSize, ChunkSize, ChunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);
    }
}
