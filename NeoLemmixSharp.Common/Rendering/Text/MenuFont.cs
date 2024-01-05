using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class MenuFont : INeoLemmixFont
{
	private const int LowerCharLimit = ' ';
	private const int UpperCharLimit = '~';

	public const int GlyphWidth = 16;
	public const int GlyphHeight = 19;

	private readonly Texture2D _texture;

	public float RenderLayer { get; set; } = 0.9f;

	public MenuFont(ContentManager content)
	{
		_texture = content.Load<Texture2D>("fonts/menu_font");
	}

	public void Dispose()
	{
		_texture.Dispose();
	}

	public void RenderText(
		SpriteBatch spriteBatch,
		ReadOnlySpan<char> charactersToRender,
		int x,
		int y,
		int scaleMultiplier,
		Color color)
	{
		var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
		foreach (var c in charactersToRender)
		{
			if (c < LowerCharLimit || c > UpperCharLimit)
				continue;

			var source = new Rectangle(GlyphWidth * (c - (1 + LowerCharLimit)), 0, GlyphWidth, GlyphHeight);
			spriteBatch.Draw(
				_texture,
				dest,
				source,
				color,
				RenderLayer);
			dest.X += GlyphWidth * scaleMultiplier;
		}
	}

	public void RenderTextSpan(
		SpriteBatch spriteBatch,
		ReadOnlySpan<int> charactersToRender,
		int x,
		int y,
		int scaleMultiplier,
		Color color)
	{
		var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
		foreach (var c in charactersToRender)
		{
			if (c < LowerCharLimit || c > UpperCharLimit)
				continue;

			var source = new Rectangle(GlyphWidth * (c - (1 + LowerCharLimit)), 0, GlyphWidth, GlyphHeight);
			spriteBatch.Draw(
				_texture,
				dest,
				source,
				color,
				RenderLayer);
			dest.X += GlyphWidth * scaleMultiplier;
		}
	}
}