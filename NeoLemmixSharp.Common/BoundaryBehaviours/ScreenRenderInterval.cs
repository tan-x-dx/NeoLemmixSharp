using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("sourceS: {SourceStart}, sourceL: {SourceLength}, screenS: {ScreenStart}, screenL: {ScreenLength}")]
public readonly struct ScreenRenderInterval
{
    public readonly int SourceStart;
    public readonly int SourceLength;
    public readonly int ScreenStart;
    public readonly int ScreenLength;

    [DebuggerStepThrough]
    public ScreenRenderInterval(int sourceStart, int sourceLength, int screenStart, int screenLength)
    {
        SourceStart = sourceStart;
        SourceLength = sourceLength;
        ScreenStart = screenStart;
        ScreenLength = screenLength;
    }
}