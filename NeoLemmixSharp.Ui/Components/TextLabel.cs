using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;
using System.Diagnostics;
using System.Numerics;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextLabel : Component
{
    private string _textLabel = string.Empty;
    private UiConstants.TextRenderMode _textRenderMode;
    private int _labelOffsetX;
    private int _labelOffsetY;

    public UiConstants.TextRenderMode TextRenderMode
    {
        get => _textRenderMode;
        set
        {
            if (_textRenderMode == value)
                return;

            _textRenderMode = value;
            if (value == UiConstants.TextRenderMode.UseFont)
            {
                UiHandler.Instance.DeregisterTextLabelForShaderRendering(this);
            }
            else
            {
                UiHandler.Instance.RegisterTextLabelForShaderRendering(this);
            }
        }
    }

    public string? Label
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

    public TextLabel(int x, int y, string label, ColorPacket colors)
        : base(x, y, 0, 0)
    {
        Label = label;

        SetSize(MenuFont.GlyphWidth * label.Length, MenuFont.GlyphHeight);

        Colors = colors;
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        if (TextRenderMode != UiConstants.TextRenderMode.UseFont)
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
        Debug.Assert(TextRenderMode == UiConstants.TextRenderMode.UseSprites);

        var colors = Colors.AsSpan();
        var color = colors.At((int)State);

        var labelX = Left + _labelOffsetX;
        var labelY = Top + _labelOffsetY;

        FontBank.MenuFont.RenderText(spriteBatch, Label, labelX, labelY, 1, color);
    }
}
