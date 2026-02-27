using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;
using System.Diagnostics;
using System.Numerics;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextLabel : Component
{
    private string _textLabel = string.Empty;
    private TextRenderMode _textRenderMode;
    private int _labelOffsetX;
    private int _labelOffsetY;

    public TextRenderMode TextRenderMode
    {
        get => _textRenderMode;
        set
        {
            if (_textRenderMode == value)
                return;

            _textRenderMode = value;
            if (value == TextRenderMode.UseFont)
            {
                UiHandler.Instance.DeregisterTextLabelForShaderRendering(this);
            }
            else
            {
                UiHandler.Instance.RegisterTextLabelForShaderRendering(this);
            }
        }
    }

    public string Label
    {
        get => _textLabel;
        set => _textLabel = value ?? string.Empty;
    }

    public int LabelOffsetX
    {
        get => _labelOffsetX;
        set => _labelOffsetX = value;
    }

    public int LabelOffsetY
    {
        get => _labelOffsetY;
        set => _labelOffsetY = value;
    }

    public TextLabel(string label)
        : this(0, 0, label)
    {
    }

    public TextLabel(int x, int y, string label)
        : base(x, y, 0, 0)
    {
        Label = label;

        SetSize(MenuFont.GlyphWidth * label.Length, MenuFont.GlyphHeight);
    }

    public override bool ContainsPoint(Point position) => false;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        if (TextRenderMode != TextRenderMode.UseFont)
            return;

        var colors = Colors.AsSpan();
        var color = colors.At((int)State);

        var labelX = Left + _labelOffsetX;
        var labelY = Top + _labelOffsetY;
        var position = new Vector2(labelX, labelY);

        spriteBatch.DrawString(
            UiSprites.UiFont,
            _textLabel,
            position,
            color);
    }

    internal void RenderMenuFont(SpriteBatch spriteBatch)
    {
        Debug.Assert(TextRenderMode == TextRenderMode.UseSprites);

        var colors = Colors.AsSpan();
        var color = colors.At((int)State);

        var labelX = Left + _labelOffsetX;
        var labelY = Top + _labelOffsetY;

        FontBank.MenuFont.RenderText(spriteBatch, Label, labelX, labelY, 1, color);
    }
}
