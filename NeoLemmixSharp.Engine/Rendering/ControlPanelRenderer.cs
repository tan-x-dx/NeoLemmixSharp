using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class ControlPanelRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelControlPanel _levelControlPanel;

    private readonly ControlPanelButtonRenderer[] _controlPanelButtonRenderers;

    private RenderTarget2D _controlPanelRenderTarget;

    private bool _disposed;

    public ControlPanelRenderer(
        GraphicsDevice graphicsDevice,
        ControlPanelSpriteBank spriteBank,
        LevelControlPanel levelControlPanel)
    {
        _graphicsDevice = graphicsDevice;
        _levelControlPanel = levelControlPanel;

        _controlPanelRenderTarget = GetControlPanelRenderTarget2D();

        var allButtons = _levelControlPanel.AllButtons;
        _controlPanelButtonRenderers = SetUpButtonRenderers(spriteBank, allButtons);
    }

    private static ControlPanelButtonRenderer[] SetUpButtonRenderers(
        ControlPanelSpriteBank spriteBank,
        ReadOnlySpan<ControlPanelButton> allButtons)
    {
        var result = new ControlPanelButtonRenderer[allButtons.Length];

        var i = 0;
        foreach (var button in allButtons)
        {
            if (button is null)
                continue;
            try
            {
                result[i++] = button.CreateButtonRenderer(spriteBank);
            }
            catch { }
        }

        return result;
    }

    public void RenderControlPanel(SpriteBatch spriteBatch)
    {
        _graphicsDevice.SetRenderTarget(_controlPanelRenderTarget);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        RenderSkillAssignButtons(spriteBatch);

        var levelTimer = _levelControlPanel.LevelTimer;
        var timerX = _levelControlPanel.Width - PanelFont.GlyphWidth * LevelTimer.NumberOfChars;

        FontBank.PanelFont.RenderTextSpan(
            spriteBatch,
            levelTimer.AsSpan(),
            timerX,
            0,
            1,
            levelTimer.FontColor);

        spriteBatch.End();
    }

    private void RenderSkillAssignButtons(SpriteBatch spriteBatch)
    {
        foreach (var controlPanelButtonRenderer in _controlPanelButtonRenderers)
        {
            if (controlPanelButtonRenderer is null) continue;
            controlPanelButtonRenderer.Render(spriteBatch);
        }
    }

    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        var destinationRectangle = new Rectangle(
            _levelControlPanel.ControlPanelX,
            _levelControlPanel.ControlPanelY,
            _levelControlPanel.ScreenWidth,
            _levelControlPanel.ScreenHeight);

        spriteBatch.Draw(_controlPanelRenderTarget, destinationRectangle, Color.White);
    }

    private RenderTarget2D GetControlPanelRenderTarget2D()
    {
        return new RenderTarget2D(
            _graphicsDevice,
            _levelControlPanel.Width,
            _levelControlPanel.Height,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void OnWindowSizeChanged()
    {
        DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
        _controlPanelRenderTarget = GetControlPanelRenderTarget2D();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);

        _disposed = true;
    }
}