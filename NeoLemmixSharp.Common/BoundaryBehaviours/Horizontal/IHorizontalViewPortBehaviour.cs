namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public interface IHorizontalViewPortBehaviourAaa
{
    // Raw pixels, one-to-one with game
    int LevelWidthInPixels { get; }
    int ViewPortX { get; }
    int ViewPortWidth { get; }

    // Stretched to fit the screen
    int ScreenX { get; }
    int ScreenWidth { get; }

    int NumberOfHorizontalRenderIntervals { get; }
    RenderInterval GetHorizontalRenderInterval(int i);

    void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth);
    void ScrollHorizontally(int dx);
    void RecalculateHorizontalRenderIntervals(int scaleMultiplier);

    ReadOnlySpan<RenderInterval> GetHorizontalRenderIntervals(Span<RenderInterval> baseSpan);
}