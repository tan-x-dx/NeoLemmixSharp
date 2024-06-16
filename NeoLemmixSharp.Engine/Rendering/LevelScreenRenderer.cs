using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelScreenRenderer : IScreenRenderer
{
    public static LevelScreenRenderer Instance { get; private set; } = null!;

    private readonly GraphicsDevice _graphicsDevice;
    private readonly Level.Viewport _viewport;

    private readonly LemmingSpriteBank _lemmingSpriteBank;
    private readonly GadgetSpriteBank _gadgetSpriteBank;

    private readonly LevelRenderer _levelRenderer;
    private readonly ControlPanelRenderer _controlPanelRenderer;
    private readonly LevelCursorSprite _levelCursorSprite;

    private DepthStencilState _depthStencilState;

    public bool IsDisposed { get; private set; }

    public LevelScreenRenderer(
        GraphicsDevice graphicsDevice,
        LevelControlPanel levelControlPanel,
        Level.Viewport viewport,
        LevelRenderer levelRenderer,
        LevelCursorSprite levelCursorSprite,
        LemmingSpriteBank lemmingSpriteBank,
        GadgetSpriteBank gadgetSpriteBank,
        ControlPanelSpriteBank controlPanelSpriteBank)
    {
        _graphicsDevice = graphicsDevice;
        _viewport = viewport;

        _lemmingSpriteBank = lemmingSpriteBank;
        _gadgetSpriteBank = gadgetSpriteBank;

        _levelRenderer = levelRenderer;

        _controlPanelRenderer = new ControlPanelRenderer(
            _graphicsDevice,
            controlPanelSpriteBank,
            levelControlPanel);

        _levelCursorSprite = levelCursorSprite;

        _depthStencilState = new DepthStencilState { DepthBufferEnable = true };
        _graphicsDevice.DepthStencilState = _depthStencilState;

        Instance = this;
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        _levelRenderer.RenderLevel(spriteBatch);

        _controlPanelRenderer.RenderControlPanel(spriteBatch);

        _graphicsDevice.SetRenderTarget(null);

        spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        //     _graphicsDevice.Clear(Color.DarkGray);
        _levelRenderer.DrawToScreen(spriteBatch);
        _controlPanelRenderer.DrawToScreen(spriteBatch);
        _levelCursorSprite.RenderAtPosition(
            spriteBatch,
            _viewport.HorizontalBoundaryBehaviour.MouseScreenCoordinate,
            _viewport.VerticalBoundaryBehaviour.MouseScreenCoordinate,
            _viewport.ScaleMultiplier);

        Span<int> foo = stackalloc int[5];
        TextRenderingHelpers.WriteDigits(foo, LevelScreen.PixelChangeCount);
        FontBank.MenuFont.RenderTextSpan(spriteBatch, foo, 20, 20, 1, Color.White);

        spriteBatch.End();
    }

    public void OnWindowSizeChanged()
    {
        _levelRenderer.OnWindowSizeChanged();
        _controlPanelRenderer.OnWindowSizeChanged();
    }

    public void AddLemmingRenderer(LemmingRenderer lemmingRenderer)
    {
        _levelRenderer.AddLemmingRenderer(lemmingRenderer);
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        DisposableHelperMethods.DisposeOf(ref _depthStencilState);
        _levelRenderer.Dispose();
        _controlPanelRenderer.Dispose();
        _lemmingSpriteBank.Dispose();
        _gadgetSpriteBank.Dispose();

        Instance = null!;

        IsDisposed = true;
    }
}