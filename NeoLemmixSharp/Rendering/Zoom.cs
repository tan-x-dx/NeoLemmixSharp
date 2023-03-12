using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Rendering;

public sealed class Zoom
{
    private const int MaxScale = 10;

    private int _previousScrollWheelValue;

    public int ScaleMultiplier { get; private set; } = 1;

    public void TrackScrollWheel(MouseState mouseState)
    {
        var delta = mouseState.ScrollWheelValue - _previousScrollWheelValue;
        _previousScrollWheelValue = mouseState.ScrollWheelValue;

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
        if (ScaleMultiplier >= MaxScale)
        {
            ScaleMultiplier = MaxScale;
        }
        else
        {
            ScaleMultiplier++;
        }
    }

    private void ZoomOut()
    {
        if (ScaleMultiplier <= 1)
        {
            ScaleMultiplier = 1;
        }
        else
        {
            ScaleMultiplier--;
        }
    }
}