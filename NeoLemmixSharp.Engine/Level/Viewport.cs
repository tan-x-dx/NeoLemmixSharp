using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class Viewport
{
	private const int MinScale = 1;
	private const int MaxScale = 12;

	private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
	private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;
	private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
	private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

	private int _windowWidth;
	private int _windowHeight;
	private int _controlPanelHeight;

	private int _scrollDelta;

	public int ScaleMultiplier { get; private set; } = 6;

	public int ViewportMouseX { get; private set; }
	public int ViewportMouseY { get; private set; }
	public int ScreenMouseX { get; private set; }
	public int ScreenMouseY { get; private set; }

	public bool MouseIsInLevelViewPort { get; private set; }

	// Raw pixels, one-to-one with game
	public int ViewPortX => _horizontalViewPortBehaviour.ViewPortX;
	public int ViewPortY => _verticalViewPortBehaviour.ViewPortY;

	// Stretched to fit the screen
	public int ScreenX => _horizontalViewPortBehaviour.ScreenX;
	public int ScreenY => _verticalViewPortBehaviour.ScreenY;
	public int ScreenWidth => _horizontalViewPortBehaviour.ScreenWidth;
	public int ScreenHeight => _verticalViewPortBehaviour.ScreenHeight;

	public int NumberOfHorizontalRenderIntervals => _horizontalViewPortBehaviour.NumberOfHorizontalRenderIntervals;
	public int NumberOfVerticalRenderIntervals => _verticalViewPortBehaviour.NumberOfVerticalRenderIntervals;

	public Viewport(
		IHorizontalViewPortBehaviour horizontalViewPortBehaviour,
		IVerticalViewPortBehaviour verticalViewPortBehaviour,
		IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
		IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
	{
		_horizontalViewPortBehaviour = horizontalViewPortBehaviour;
		_verticalViewPortBehaviour = verticalViewPortBehaviour;
		_horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
		_verticalBoundaryBehaviour = verticalBoundaryBehaviour;

		_scrollDelta = MaxScale / ScaleMultiplier;
	}

	public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight, int controlPanelHeight)
	{
		_windowWidth = gameWindowWidth;
		_windowHeight = gameWindowHeight;
		_controlPanelHeight = controlPanelHeight;

		_horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
		_horizontalViewPortBehaviour.ScrollHorizontally(0);
		_horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
		_verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight, _controlPanelHeight);
		_verticalViewPortBehaviour.ScrollVertically(0);
		_verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
	}

	public void HandleMouseInput(LevelInputController inputController)
	{
		ScreenMouseX = ScaleMultiplier * ((inputController.MouseX + ScaleMultiplier / 2) / ScaleMultiplier);
		ScreenMouseY = ScaleMultiplier * ((inputController.MouseY + ScaleMultiplier / 2) / ScaleMultiplier);

		if (MouseIsInLevelViewport(inputController))
		{
			MouseIsInLevelViewPort = true;
			TrackScrollWheel(inputController);
		}
		else
		{
			MouseIsInLevelViewPort = false;
		}

		ViewportMouseX = (ScreenMouseX - _horizontalViewPortBehaviour.ScreenX) / ScaleMultiplier + _horizontalViewPortBehaviour.ViewPortX;
		ViewportMouseY = (ScreenMouseY - _verticalViewPortBehaviour.ScreenY) / ScaleMultiplier + _verticalViewPortBehaviour.ViewPortY;

		ViewportMouseX = _horizontalBoundaryBehaviour.NormaliseX(ViewportMouseX);
		ViewportMouseY = _verticalBoundaryBehaviour.NormaliseY(ViewportMouseY);

		if (inputController.MouseX == 0)
		{
			_horizontalViewPortBehaviour.ScrollHorizontally(-_scrollDelta);
			_horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
		}
		else if (inputController.MouseX == _windowWidth - 1)
		{
			_horizontalViewPortBehaviour.ScrollHorizontally(_scrollDelta);
			_horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
		}

		if (inputController.MouseY == 0)
		{
			_verticalViewPortBehaviour.ScrollVertically(-_scrollDelta);
			_verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
		}
		else if (inputController.MouseY == _windowHeight - 1)
		{
			_verticalViewPortBehaviour.ScrollVertically(_scrollDelta);
			_verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool MouseIsInLevelViewport(LevelInputController inputController)
	{
		return inputController.MouseX >= 0 && inputController.MouseX <= _windowWidth &&
			   inputController.MouseY >= 0 && inputController.MouseY <= _windowHeight - _controlPanelHeight;
	}

	private void TrackScrollWheel(LevelInputController inputController)
	{
		var previousValue = ScaleMultiplier;
		ScaleMultiplier = Math.Clamp(ScaleMultiplier + inputController.ScrollDelta, MinScale, MaxScale);

		if (ScaleMultiplier == previousValue)
			return;

		_horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
		_horizontalViewPortBehaviour.ScrollHorizontally(0);
		_horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
		_verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight, _controlPanelHeight);
		_verticalViewPortBehaviour.ScrollVertically(0);
		_verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);

		_scrollDelta = MaxScale / ScaleMultiplier;
	}

	public RenderInterval GetHorizontalRenderInterval(int i) => _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
	public RenderInterval GetVerticalRenderInterval(int i) => _verticalViewPortBehaviour.GetVerticalRenderInterval(i);
}