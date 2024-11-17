using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class TextureButton : Button
{
    private readonly Texture2D _texture;

    public float ScaleMulitplier { get; set; } = 1f;

    public TextureButton(int x, int y, Texture2D texture)
        : base(x, y, texture.Width, texture.Height, null)
    {
        _texture = texture;
    }

    public override int Width
    {
        get => _texture.Width;
        set => throw new InvalidOperationException("Button width is determined by the texture. Use ScaleMultiplier instead.");
    }

    public override int Height
    {
        get => _texture.Height;
        set => throw new InvalidOperationException("Button height is determined by the texture. Use ScaleMultiplier instead.");
    }

    public int ScaledWidth => (int)(0.5f + Width * ScaleMulitplier);
    public int ScaledHeight => (int)(0.5f + Height * ScaleMulitplier);

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var dest = new Rectangle(
            Left,
            Top,
            ScaledWidth,
            ScaledHeight);

        spriteBatch.Draw(
            _texture,
            dest,
            Color.White);
    }
}
