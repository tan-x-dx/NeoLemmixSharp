using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Data;

public static class UiSprites
{
    internal static SpriteFont Font { get; private set; } = null!;
    internal static Texture2D BevelTexture { get; private set; } = null!;

    public static void Initialise(ContentManager contentManager)
    {
        Font = contentManager.Load<SpriteFont>("MonospacedFont");
        BevelTexture = contentManager.Load<Texture2D>("bevel");
    }

    internal static void DrawBeveledRectangle(
        SpriteBatch spriteBatch,
        RectangularComponent c)
    {
        var dest = new Rectangle(
            c.Left,
            c.Top,
            c.Width,
            c.Height);

        var color = c.Colors.NormalColor;
        DrawNineSlicedBeveledRectangle(spriteBatch, dest, color);
    }

    internal static void DrawBeveledRectangle<TComponent>(
        SpriteBatch spriteBatch,
        TComponent c)
        where TComponent : Component, IColorable, IModifyableState
    {
        var dest = new Rectangle(
            c.Left,
            c.Top,
            c.Width,
            c.Height);

        var colors = c.Colors.AsSpan();
        var color = colors[(int)c.State];

        DrawNineSlicedBeveledRectangle(spriteBatch, dest, color);
    }

    private static void DrawNineSlicedBeveledRectangle(
        SpriteBatch spriteBatch,
        Rectangle dest,
        Color color)
    {
        const int sourceSize = 64;
        const int chunkSize = 4;

        var source = new Rectangle(0, 0, chunkSize, chunkSize);
        var subDest = new Rectangle(dest.X, dest.Y, chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(chunkSize, 0, sourceSize - 2 * chunkSize, chunkSize);
        subDest = new Rectangle(dest.X + chunkSize, dest.Y, dest.Width - 2 * chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(sourceSize - chunkSize, 0, chunkSize, chunkSize);
        subDest = new Rectangle(dest.Right - chunkSize, dest.Y, chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);


        source = new Rectangle(0, chunkSize, chunkSize, sourceSize - 2 * chunkSize);
        subDest = new Rectangle(dest.X, dest.Y + chunkSize, chunkSize, dest.Height - 2 * chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(chunkSize, chunkSize, sourceSize - 2 * chunkSize, sourceSize - 2 * chunkSize);
        subDest = new Rectangle(dest.X + chunkSize, dest.Y + chunkSize, dest.Width - 2 * chunkSize, dest.Height - 2 * chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(sourceSize - chunkSize, chunkSize, chunkSize, sourceSize - 2 * chunkSize);
        subDest = new Rectangle(dest.Right - chunkSize, dest.Y + chunkSize, chunkSize, dest.Height - 2 * chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);


        source = new Rectangle(0, sourceSize - chunkSize, chunkSize, chunkSize);
        subDest = new Rectangle(dest.X, dest.Bottom - chunkSize, chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(chunkSize, sourceSize - chunkSize, sourceSize - 2 * chunkSize, chunkSize);
        subDest = new Rectangle(dest.X + chunkSize, dest.Bottom - chunkSize, dest.Width - 2 * chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);

        source = new Rectangle(sourceSize - chunkSize, sourceSize - chunkSize, chunkSize, chunkSize);
        subDest = new Rectangle(dest.Right - chunkSize, dest.Bottom - chunkSize, chunkSize, chunkSize);
        spriteBatch.Draw(BevelTexture, subDest, source, color);
    }
}
