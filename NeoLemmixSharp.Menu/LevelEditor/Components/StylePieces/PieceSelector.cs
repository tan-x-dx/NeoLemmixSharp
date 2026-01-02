using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;
using Size = NeoLemmixSharp.Common.Size;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.StylePieces;

public sealed class PieceSelector : Component
{
    public const int BaseSpriteRenderDimension = 80;
    private const int TileBorderSize = 2;

    public IArchetypeData StylePiece { get; }
    private readonly Texture2D _sourceTexture;

    public PieceSelector(IArchetypeData piece) : base(0, 0, BaseSpriteRenderDimension, BaseSpriteRenderDimension, piece.PieceIdentifier.ToString())
    {
        StylePiece = piece;
        _sourceTexture = TextureCache.GetOrLoadTexture(piece.TextureFilePath, piece.StyleIdentifier, piece.PieceIdentifier, piece.TextureType);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if (!IsVisible)
            return;

        RenderTile(spriteBatch);

        DetermineRenderRectangles(out var sourceRectangle, out var destinationRectangle);

        destinationRectangle.X += Left;
        destinationRectangle.Y += Top;

        spriteBatch.Draw(
            _sourceTexture,
            destinationRectangle,
            sourceRectangle,
            Color.White);

        spriteBatch.DrawString(
            UiSprites.UiFont,
            StylePiece.Name,
            new Vector2(Left + 2f, Bottom - 18f),
            Color.White,
            0f,
            Vector2.Zero,
            UiConstants.FontScaleFactor,
            SpriteEffects.None,
            1.0f);
    }

    private void RenderTile(SpriteBatch spriteBatch)
    {
        var p = Position - new Point(TileBorderSize, TileBorderSize);
        var s = new Size(BaseSpriteRenderDimension + TileBorderSize, BaseSpriteRenderDimension + TileBorderSize);
        var fillRectangle = Helpers.CreateRectangle(p, s);

        spriteBatch.FillRect(fillRectangle, new Color(0xff696969));

        p = Position;
        s = new Size(BaseSpriteRenderDimension, BaseSpriteRenderDimension);
        fillRectangle = Helpers.CreateRectangle(p, s);

        spriteBatch.FillRect(fillRectangle, Color.Black);
    }

    private void DetermineRenderRectangles(
        out Rectangle sourceRectangle,
        out Rectangle destinationRectangle)
    {
        DetermineRenderSizes(_sourceTexture.Width, out var sourceIntervalH, out var destinationIntervalH);
        DetermineRenderSizes(_sourceTexture.Height, out var sourceIntervalV, out var destinationIntervalV);

        sourceRectangle = new Rectangle(
            sourceIntervalH.Start,
            sourceIntervalV.Start,
            sourceIntervalH.Length,
            sourceIntervalV.Length);
        destinationRectangle = new Rectangle(
            destinationIntervalH.Start,
            destinationIntervalV.Start,
            destinationIntervalH.Length,
            destinationIntervalV.Length);

        return;

        static void DetermineRenderSizes(int textureLength, out Interval sourceInterval, out Interval destinationInterval)
        {
            var delta = (textureLength - BaseSpriteRenderDimension) / 2;

            if (textureLength <= BaseSpriteRenderDimension)
            {
                delta = -delta;
                sourceInterval = new Interval(0, textureLength);
                destinationInterval = new Interval(delta, textureLength);
            }
            else
            {
                sourceInterval = new Interval(delta, BaseSpriteRenderDimension);
                destinationInterval = new Interval(0, BaseSpriteRenderDimension);
            }
        }
    }
}
