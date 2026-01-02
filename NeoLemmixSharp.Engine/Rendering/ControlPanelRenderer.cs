using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class ControlPanelRenderer : IDisposable
{
    public const int TotalNumberOfIcons = 7;
    public const int PanelIconWidth = 8;
    private const int PanelIconHeight = 16;

    private const int ReplayPanelIconX = 0 * PanelIconWidth;
    private const int HatchPanelIconX = 1 * PanelIconWidth;
    private const int LemmingPanelIconX = 2 * PanelIconWidth;
    private const int FlagPanelIconX = 3 * PanelIconWidth;
    private const int TimerUpPanelIconX = 4 * PanelIconWidth;
    private const int TimerDownPanelIconX = 5 * PanelIconWidth;
    private const int EditReplayPanelIconX = 6 * PanelIconWidth;

    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelControlPanel _levelControlPanel;

    private readonly ControlPanelButtonRenderer[] _controlPanelButtonRenderers;
    private readonly Texture2D _panelIconsTexture;
    private readonly Texture2D _minimapRegionTexture;

    private RenderTarget2D _controlPanelRenderTarget;

    private Size _windowSize;

    private bool _isDisposed;

    public ControlPanelRenderer(
        GraphicsDevice graphicsDevice,
        ControlPanelSpriteBank controlPanelSpriteBank,
        LevelControlPanel levelControlPanel)
    {
        _graphicsDevice = graphicsDevice;
        _levelControlPanel = levelControlPanel;

        var allButtons = _levelControlPanel.AllButtons;
        _controlPanelButtonRenderers = SetUpButtonRenderers(controlPanelSpriteBank, allButtons);
        _panelIconsTexture = controlPanelSpriteBank.PanelIcons;
        _minimapRegionTexture = controlPanelSpriteBank.PanelMinimapRegion;

        _controlPanelRenderTarget = CreateControlPanelRenderTarget2D();
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
            catch
            {
            }
        }

        return result;
    }

    public void RenderControlPanel(SpriteBatch spriteBatch)
    {
        _graphicsDevice.SetRenderTarget(_controlPanelRenderTarget);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        RenderSkillAssignButtons(spriteBatch);
        RenderTextualDataAndIcons(spriteBatch);

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

    private void RenderTextualDataAndIcons(SpriteBatch spriteBatch)
    {
        var textualData = _levelControlPanel.TextualData;

        var x = 0;

        var currentTextSpan = textualData.LemmingActionAndCountSpan;
        RenderText(currentTextSpan, x);
        var dx = currentTextSpan.Length * PanelFont.GlyphWidth;
        x += dx;

        RenderIcon(HatchPanelIconX, x);
        x += PanelIconWidth;

        currentTextSpan = textualData.HatchCountSpan;
        RenderText(currentTextSpan, x);
        dx = (currentTextSpan.Length + 1) * PanelFont.GlyphWidth;
        x += dx;

        RenderIcon(LemmingPanelIconX, x);
        x += PanelIconWidth;

        currentTextSpan = textualData.LemmingsOutSpan;
        RenderText(currentTextSpan, x);
        dx = (currentTextSpan.Length + 1) * PanelFont.GlyphWidth;
        x += dx;

        RenderIcon(FlagPanelIconX, x);
        x += PanelIconWidth;

        currentTextSpan = textualData.GoalCountSpan;
        RenderText(currentTextSpan, x);
        dx = (currentTextSpan.Length + 1) * PanelFont.GlyphWidth;
        x += dx;

        var levelTimer = LevelScreen.LevelTimer;
        var timerIconX = levelTimer.Type == TimerType.CountDown
            ? TimerDownPanelIconX
            : TimerUpPanelIconX;
        RenderIcon(timerIconX, x);
        x += PanelIconWidth;

        currentTextSpan = levelTimer.AsReadOnlySpan();
        RenderText(currentTextSpan, x);

        return;

        void RenderText(ReadOnlySpan<char> span, int renderX)
        {
            FontBank.PanelFont.RenderTextSpan(
                spriteBatch,
                span,
                renderX,
                0,
                1,
                EngineConstants.PanelGreen);
        }

        void RenderIcon(int iconX, int renderX)
        {
            var destinationRectangle = new Rectangle(renderX, 0, PanelIconWidth, PanelIconHeight);
            var sourceRectangle = new Rectangle(iconX, 0, PanelIconWidth, PanelIconHeight);

            spriteBatch.Draw(
                _panelIconsTexture,
                destinationRectangle,
                sourceRectangle,
                Color.White);
        }
    }

    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        var controlPanelScreenSize = _levelControlPanel.ControlPanelScreenSize;
        spriteBatch.FillRect(
            new Rectangle(0, _windowSize.H - controlPanelScreenSize.H, _windowSize.W, controlPanelScreenSize.H),
            Color.DarkGray);

        var controlPanelPosition = _levelControlPanel.ControlPanelPosition;
        var destinationRectangle = Helpers.CreateRectangle(
            controlPanelPosition,
            controlPanelScreenSize);

        spriteBatch.Draw(_controlPanelRenderTarget, destinationRectangle, Color.White);
    }

    private RenderTarget2D CreateControlPanelRenderTarget2D()
    {
        var controlPanelSize = _levelControlPanel.ControlPanelSize;
        return new RenderTarget2D(
            _graphicsDevice,
            controlPanelSize.W,
            controlPanelSize.H,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void OnWindowSizeChanged()
    {
        _windowSize = IGameWindow.Instance.WindowSize;

        DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
        _controlPanelRenderTarget = CreateControlPanelRenderTarget2D();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
        }

        GC.SuppressFinalize(this);
    }
}
