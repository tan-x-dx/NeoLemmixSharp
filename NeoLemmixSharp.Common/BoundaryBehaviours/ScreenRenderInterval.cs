using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("sourceC: {SourceCoordinate}, sourceD: {SourceDimension}, screenC: {ScreenCoordinate}, screenD: {ScreenDimension}")]
public readonly struct ScreenRenderInterval
{
    public readonly int SourceCoordinate;
    public readonly int SourceDimension;
    public readonly int ScreenCoordinate;
    public readonly int ScreenDimension;

    [DebuggerStepThrough]
    public ScreenRenderInterval(int sourceCoordinate, int sourceDimension, int screenCoordinate, int screenDimension)
    {
        SourceCoordinate = sourceCoordinate;
        SourceDimension = sourceDimension;
        ScreenCoordinate = screenCoordinate;
        ScreenDimension = screenDimension;
    }
}