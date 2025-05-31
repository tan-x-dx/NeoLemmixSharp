using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingSpriteData
{
    internal readonly TribeColorData[] _tribeColorData = new TribeColorData[EngineConstants.MaxNumberOfTribes];
    internal readonly LemmingActionSpriteData[] _lemmingActionSpriteData = new LemmingActionSpriteData[LemmingActionConstants.NumberOfLemmingActions];

    public StyleIdentifier LemmingSpriteStyleIdentifier { get; }

    public LemmingSpriteData(StyleIdentifier lemmingSpriteStyleIdentifier)
    {
        LemmingSpriteStyleIdentifier = lemmingSpriteStyleIdentifier;
    }

    public ReadOnlySpan<LemmingActionSpriteData> LemmingActionSpriteData => _lemmingActionSpriteData;
    public ReadOnlySpan<TribeColorData> TribeColorData => _tribeColorData;
}
