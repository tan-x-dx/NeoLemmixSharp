using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;
using Size = NeoLemmixSharp.Common.Size;

namespace NeoLemmixSharp.Menu.LevelEditor.Components;

public sealed class PieceSelector : Component
{
    public const int BaseSpriteRenderDimension = 64;

    public IArchetypeData StylePiece { get; }
    private readonly Texture2D _sourceTexture;

    public PieceSelector(IArchetypeData piece) : base(0, 0, BaseSpriteRenderDimension, BaseSpriteRenderDimension, piece.PieceIdentifier.ToString())
    {
        StylePiece = piece;
        _sourceTexture = TextureCache.GetOrLoadTexture(piece.TextureFilePath, piece.StyleIdentifier, piece.PieceIdentifier, piece.TextureType);
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var sourceSize = GetRenderSize();
        var destinationRectangle = Helpers.CreateRectangle(Position, sourceSize);

        spriteBatch.FillRect(destinationRectangle, Color.Black);

        var sourceRectangle = Helpers.CreateRectangle(new Point(), sourceSize);

        spriteBatch.Draw(
            _sourceTexture,
            destinationRectangle,
            sourceRectangle,
            Color.White);
    }

    private Size GetRenderSize()
    {
        var w = Math.Min(_sourceTexture.Width, BaseSpriteRenderDimension);
        var h = Math.Min(_sourceTexture.Height, BaseSpriteRenderDimension);

        return new Size(w, h);
    }
}
