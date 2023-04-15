namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public interface IHorizontalViewPortBehaviour
{
    // Raw pixels, one-to-one with game
    int ViewPortX { get; }
    int ViewPortWidth { get; }

    // Stretched to fit the screen
    int ScreenX { get; }
    int ScreenWidth { get; }
    
    RenderInterval[] HorizontalRenderIntervals { get; }

    int NormaliseX(int x);
    void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth);
    void ScrollHorizontally(int dx);
    void RecalculateHorizontalRenderIntervals(int scaleMultiplier);
}