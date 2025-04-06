using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class TextureButton : Component
{
    private readonly Texture2D _texture;

    public float ScaleMultiplier { get; set; } = 1f;

    public TextureButton(int x, int y, Texture2D texture)
        : base(x, y, texture.Width, texture.Height, null)
    {
        _texture = texture;
    }

    public override int Width
    {
        get => ScaledWidth;
        set { }
    }

    public override int Height
    {
        get => ScaledHeight;
        set { }
    }

    public int ScaledWidth => (int)(0.5f + _texture.Width * ScaleMultiplier);
    public int ScaledHeight => (int)(0.5f + _texture.Height * ScaleMultiplier);

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
