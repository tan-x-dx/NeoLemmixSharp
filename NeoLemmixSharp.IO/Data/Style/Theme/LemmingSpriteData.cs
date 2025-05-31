using NeoLemmixSharp.Common;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingSpriteData
{
    public StyleIdentifier StyleIdentifier { get; }

    public LemmingSpriteData(StyleIdentifier styleIdentifier)
    {
        StyleIdentifier = styleIdentifier;
    }

    public LemmingActionSpriteDataBuffer LemmingActionSpriteData = new();
    public TribeColorData[] TribeColorData { get; } = new TribeColorData[EngineConstants.MaxNumberOfTribes];

    [InlineArray(LemmingActionConstants.NumberOfLemmingActions)]
    public struct LemmingActionSpriteDataBuffer
    {
        private LemmingActionSpriteData _0;
    }
}
