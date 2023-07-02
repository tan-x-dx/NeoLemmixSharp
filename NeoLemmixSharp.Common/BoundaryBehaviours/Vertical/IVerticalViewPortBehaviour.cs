namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public interface IVerticalViewPortBehaviour
{
    // Raw pixels, one-to-one with game
    int LevelHeightInPixels { get; }
    int ViewPortY { get; }
    int ViewPortHeight { get; }

    // Stretched to fit the screen
    int ScreenY { get; }
    int ScreenHeight { get; }

    int NumberOfVerticalRenderIntervals { get; }
    RenderInterval GetVerticalRenderInterval(int i);

    void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight, int controlPanelHeight);
    void ScrollVertically(int dy);
    void RecalculateVerticalRenderIntervals(int scaleMultiplier);
}
