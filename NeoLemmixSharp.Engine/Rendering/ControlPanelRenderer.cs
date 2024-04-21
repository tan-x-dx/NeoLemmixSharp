using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class ControlPanelRenderer
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
    private readonly Texture2D _whitePixel;
    private readonly Texture2D _panelIconsTexture;
    private readonly Texture2D _minimapRegionTexture;

    private RenderTarget2D _controlPanelRenderTarget;

    private int _windowWidth;
    private int _windowHeight;

    private bool _disposed;

    public ControlPanelRenderer(
        GraphicsDevice graphicsDevice,
        ControlPanelSpriteBank controlPanelSpriteBank,
        LevelControlPanel levelControlPanel)
    {
        _graphicsDevice = graphicsDevice;
        _levelControlPanel = levelControlPanel;

        var allButtons = _levelControlPanel.AllButtons;
        _controlPanelButtonRenderers = SetUpButtonRenderers(controlPanelSpriteBank, allButtons);
        _whitePixel = CommonSprites.WhitePixelGradientSprite;
        _panelIconsTexture = controlPanelSpriteBank.PanelIcons;
        _minimapRegionTexture = controlPanelSpriteBank.PanelMinimapRegion;

        _controlPanelRenderTarget = GetControlPanelRenderTarget2D();
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

        var lemmingActionAndCountSpan = textualData.LemmingActionAndCountSpan;
        RenderText(lemmingActionAndCountSpan, x);
        x += lemmingActionAndCountSpan.Length * PanelFont.GlyphWidth;

        RenderIcon(HatchPanelIconX, x);
        x += PanelIconWidth;

        var hatchCountSpan = textualData.HatchCountSpan;
        RenderText(hatchCountSpan, x);
        x += (hatchCountSpan.Length + 1) * PanelFont.GlyphWidth;

        RenderIcon(LemmingPanelIconX, x);
        x += PanelIconWidth;

        var lemmingOutSpan = textualData.LemmingsOutSpan;
        RenderText(lemmingOutSpan, x);
        x += (lemmingOutSpan.Length + 1) * PanelFont.GlyphWidth;

        RenderIcon(FlagPanelIconX, x);
        x += PanelIconWidth;

        var goalCountSpan = textualData.GoalCountSpan;
        RenderText(goalCountSpan, x);
        x += (goalCountSpan.Length + 1) * PanelFont.GlyphWidth;

        var levelTimer = textualData.LevelTimer;
        var timerIconX = levelTimer.Type == LevelTimer.TimerType.CountDown
            ? TimerDownPanelIconX
            : TimerUpPanelIconX;
        RenderIcon(timerIconX, x);
        x += PanelIconWidth;

        var timerSpan = levelTimer.AsSpan();
        RenderText(timerSpan, x);

        return;

        void RenderText(ReadOnlySpan<int> span, int renderX)
        {
            FontBank.PanelFont.RenderTextSpan(
                spriteBatch,
                span,
                renderX,
                0,
                1,
                LevelConstants.PanelGreen);
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
        var controlPanelScreenHeight = _levelControlPanel.ScreenHeight;
        spriteBatch.Draw(
            _whitePixel,
            new Rectangle(0, _windowHeight - controlPanelScreenHeight, _windowWidth, controlPanelScreenHeight),
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            Color.DarkGray);

        var destinationRectangle = new Rectangle(
            _levelControlPanel.ControlPanelX,
            _levelControlPanel.ControlPanelY,
            _levelControlPanel.ScreenWidth,
            controlPanelScreenHeight);

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
        var gameWindow = IGameWindow.Instance;
        _windowWidth = gameWindow.WindowWidth;
        _windowHeight = gameWindow.WindowHeight;

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