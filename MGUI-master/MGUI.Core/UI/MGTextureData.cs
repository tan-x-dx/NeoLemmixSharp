using MGUI.Shared.Helpers;
using MGUI.Shared.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MGUI.Core.UI;

/// <param name="RenderSizeOverride">The <see cref="Size"/> to use for the destination <see cref="Rectangle"/> when drawing this texture via <see cref="Draw(DrawTransaction, Point, Color?, float)"/>.<para/>
/// See also: <see cref="RenderSize"/></param>
public readonly record struct MGTextureData(Texture2D Texture, Rectangle? SourceRect = null, float Opacity = 1f, Size? RenderSizeOverride = null)
{
    /// <summary>The actual size this texture will be drawn at if using <see cref="Draw(DrawTransaction, Point, Microsoft.Xna.Framework.Color?, float)"/></summary>
    public Size RenderSize => RenderSizeOverride ?? SourceRect?.Size.AsSize() ?? new Size(Texture.Width, Texture.Height);

    private static Rectangle GetRectangle(Point topleft, Size size) => new(topleft.X, topleft.Y, size.Width, size.Height);

    public void Draw(DrawTransaction dt, Point position, Color? color = null, float opacity = 1f)
        => Draw(dt, GetRectangle(position, RenderSize), color, opacity);
    public void Draw(DrawTransaction dt, Rectangle destination, Color? color = null, float opacity = 1f)
        => dt.DrawTextureTo(Texture, SourceRect, destination, (color ?? Color.White) * Opacity * opacity);
}
