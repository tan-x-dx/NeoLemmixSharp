using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Widgets;

public sealed class TextureButton : Button
{
    private readonly Texture2D _texture;

    public TextureButton(Texture2D texture)
    {
        _texture = texture;
    }
}