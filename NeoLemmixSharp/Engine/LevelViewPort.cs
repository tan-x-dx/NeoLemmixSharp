using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MaxNumberOfRenderTilings = 5;

    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly int _levelWidth;
    private readonly int _levelHeight;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private readonly Interval[] _horizontalTilingIntervals = new Interval[MaxNumberOfRenderTilings];
    private readonly Interval[] _verticalTilingIntervals = new Interval[MaxNumberOfRenderTilings];

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    private int _screenTileX;
    private int _screenTileY;

    private int _scrollDelta;

    // Raw pixels, one-to-one with game
    public int ViewPortX => _horizontalViewPortBehaviour.ViewPortX;
    public int ViewPortY => _verticalViewPortBehaviour.ViewPortY;
    public int ViewPortWidth => _horizontalViewPortBehaviour.ViewPortWidth;
    public int ViewPortHeight => _verticalViewPortBehaviour.ViewPortHeight;

    // Stretched to fit the screen
    public int ScreenX => _horizontalViewPortBehaviour.ScreenX;
    public int ScreenY => _verticalViewPortBehaviour.ScreenY;
    public int ScreenWidth => _horizontalViewPortBehaviour.ScreenWidth;
    public int ScreenHeight => _verticalViewPortBehaviour.ScreenHeight;

    public LevelViewPort(PixelManager terrain)
    {
        _levelWidth = terrain.Width;
        _levelHeight = terrain.Height;

        _horizontalViewPortBehaviour = terrain.HorizontalViewPortBehaviour;
        _verticalViewPortBehaviour = terrain.VerticalViewPortBehaviour;

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    public int ScaleMultiplier { get; private set; } = 6;

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;

        RecalculateDimensions();
        RecalculateHorizontalScreenTiling();
        RecalculateVerticalScreenTiling();
    }

    public void HandleMouseInput(MouseState mouseState)
    {
        TrackScrollWheel(mouseState.ScrollWheelValue);

        if (mouseState.X == 0)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(-_scrollDelta);
            RecalculateHorizontalScreenTiling();
        }
        else if (mouseState.X == _windowWidth - 1)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(_scrollDelta);
            RecalculateHorizontalScreenTiling();
        }

        if (mouseState.Y == 0)
        {
            _verticalViewPortBehaviour.ScrollVertically(-_scrollDelta);
            RecalculateVerticalScreenTiling();
        }
        else if (mouseState.Y == _windowHeight - 1)
        {
            _verticalViewPortBehaviour.ScrollVertically(_scrollDelta);
            RecalculateVerticalScreenTiling();
        }
    }

    private void TrackScrollWheel(int scrollWheelValue)
    {
        var delta = scrollWheelValue - _previousScrollWheelValue;
        _previousScrollWheelValue = scrollWheelValue;

        if (delta > 0)
        {
            ZoomIn();
        }
        else if (delta < 0)
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        var previousValue = ScaleMultiplier;
        if (ScaleMultiplier < MaxScale)
        {
            ScaleMultiplier++;
        }
        else
        {
            ScaleMultiplier = MaxScale;
        }

        if (ScaleMultiplier == previousValue)
            return;

        RecalculateDimensions();
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _verticalViewPortBehaviour.ScrollVertically(0);
        RecalculateHorizontalScreenTiling();
        RecalculateVerticalScreenTiling();

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    private void ZoomOut()
    {
        var previousValue = ScaleMultiplier;
        if (ScaleMultiplier > MinScale)
        {
            ScaleMultiplier--;
        }
        else
        {
            ScaleMultiplier = MinScale;
        }

        if (ScaleMultiplier == previousValue)
            return;

        RecalculateDimensions();
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _verticalViewPortBehaviour.ScrollVertically(0);
        RecalculateHorizontalScreenTiling();
        RecalculateVerticalScreenTiling();

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    private void RecalculateDimensions()
    {
        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight);
    }

    private void RecalculateHorizontalScreenTiling()
    {
        _screenTileX = 1;//_horizontalViewPortBehaviour.GetNumberOfHorizontalRepeats();

        if (_screenTileX == 1)
        {
            _horizontalTilingIntervals[0] = new Interval(ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);
        }
        else
        {
            _horizontalTilingIntervals[0] = new Interval(ViewPortX, _levelWidth - ViewPortX, 0, ScaleMultiplier * (_levelWidth - ViewPortX));
            var x = 0;
            for (var i = 1; i < _screenTileX - 1; i++)
            {
                x += _horizontalTilingIntervals[i - 1].DestB;
                _horizontalTilingIntervals[i] = new Interval(0, _levelWidth, x, _levelWidth * ScaleMultiplier);
            }
            x += _horizontalTilingIntervals[_screenTileX - 2].DestB;
            _horizontalTilingIntervals[_screenTileX - 1] = new Interval(0, _screenTileX * _levelWidth + ViewPortX - ViewPortWidth, x, 200);
        }
    }

    private void RecalculateVerticalScreenTiling()
    {
        _screenTileY = 1;//_verticalViewPortBehaviour.GetNumberOfVerticalRepeats();

        if (_screenTileY == 1)
        {
            _verticalTilingIntervals[0] = new Interval(ViewPortY, ViewPortHeight, ScreenY, ScreenHeight);
        }
        else
        {
            _verticalTilingIntervals[0] = new Interval(ViewPortY, _levelHeight - ViewPortY, 0, ScaleMultiplier * (_levelHeight - ViewPortY));
            var y = 0;
            for (var i = 1; i < _screenTileY - 1; i++)
            {
                y += _verticalTilingIntervals[i - 1].DestB;
                _verticalTilingIntervals[i] = new Interval(0, _levelHeight, y, _levelHeight * ScaleMultiplier);
            }
            y += _verticalTilingIntervals[_screenTileY - 2].DestB;
            _verticalTilingIntervals[_screenTileY - 1] = new Interval(0, _screenTileY * _levelHeight + ViewPortY - ViewPortHeight, y, 200);
        }
    }

    public bool GetRenderDestinationRectangle(Rectangle rectangle, out Rectangle renderDestination)
    {
        if (rectangle.X < ViewPortX + ViewPortWidth &&
            ViewPortX < rectangle.X + rectangle.Width &&
            rectangle.Y < ViewPortY + ViewPortHeight &&
            ViewPortY < rectangle.Y + rectangle.Height)
        {
            var x0 = (rectangle.X - ViewPortX) * ScaleMultiplier + ScreenX;
            var y0 = (rectangle.Y - ViewPortY) * ScaleMultiplier + ScreenY;

            renderDestination = new Rectangle(
                x0,
                y0,
                rectangle.Width * ScaleMultiplier,
                rectangle.Height * ScaleMultiplier);
            return true;
        }

        renderDestination = Rectangle.Empty;
        return false;
    }

    public void RenderTerrain(SpriteBatch spriteBatch, Texture2D texture)
    {
        /*for (var i = 0; i < _screenTileX; i++)
        {
            var hInterval = _horizontalTilingIntervals[i];
            for (var j = 0; j < _screenTileY; j++)
            {
                var vInterval = _verticalTilingIntervals[j];
                var sourceRect = new Rectangle(hInterval.SourceA, vInterval.SourceA, hInterval.SourceB, vInterval.SourceB);
                var screenRect = new Rectangle(hInterval.DestA, vInterval.DestA, hInterval.DestB, vInterval.DestB);

                spriteBatch.Draw(texture, screenRect, sourceRect, Color.White);
            }
        }*/









        for (var i = 0; i < _screenTileX; i++)
        {
            for (var j = 0; j < _screenTileY; j++)
            {
                spriteBatch.Draw(
                    texture,
                    new Rectangle((i + 1) * ScreenX, (j + 1) * ScreenY, ScreenWidth, ScreenHeight),
                    Color.White);
            }
        }
    }

    private readonly struct Interval
    {
        public readonly int SourceA;
        public readonly int SourceB;

        public readonly int DestA;
        public readonly int DestB;

        public Interval(int sourceA, int sourceB, int destA, int destB)
        {
            SourceA = sourceA;
            SourceB = sourceB;
            DestA = destA;
            DestB = destB;
        }
    }

    /* var viewport = LevelScreen.CurrentLevel.Viewport;

     spriteBatch.Draw(
         _texture,
         new Rectangle(viewport.ScreenX, viewport.ScreenY, viewport.ScreenWidth, viewport.ScreenHeight),
         new Rectangle(viewport.ViewPortX, viewport.ViewPortY, viewport.ViewPortWidth, viewport.ViewPortHeight),
         Color.White);*/
}