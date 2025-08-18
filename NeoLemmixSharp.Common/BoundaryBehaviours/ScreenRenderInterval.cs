using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("sourceS: {SourceStart}, sourceL: {SourceLength}, screenS: {ScreenStart}, screenL: {ScreenLength}")]
[method: DebuggerStepThrough]
public readonly struct ScreenRenderInterval(int sourceStart, int sourceLength, int screenStart, int screenLength)
{
    public readonly int SourceStart = sourceStart;
    public readonly int SourceLength = sourceLength;
    public readonly int ScreenStart = screenStart;
    public readonly int ScreenLength = screenLength;
}
