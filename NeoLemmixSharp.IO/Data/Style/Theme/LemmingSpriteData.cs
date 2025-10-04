using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingSpriteData
{
    private readonly TribeColorData[] _tribeColorData;
    private readonly LemmingActionSpriteData[] _lemmingActionSpriteData;

    public StyleIdentifier LemmingSpriteStyleIdentifier { get; }

    internal LemmingSpriteData(
        StyleIdentifier lemmingSpriteStyleIdentifier,
        TribeColorData[] tribeColorData,
        LemmingActionSpriteData[] lemmingActionSpriteData)
    {
        LemmingSpriteStyleIdentifier = lemmingSpriteStyleIdentifier;
        _tribeColorData = tribeColorData;
        _lemmingActionSpriteData = lemmingActionSpriteData;
    }

    internal LemmingActionSpriteData[] CloneLemmingActionSpriteData()
    {
        var result = new LemmingActionSpriteData[LemmingActionConstants.NumberOfLemmingActions];

        LemmingActionSpriteData.CopyTo(result);

        return result;
    }

    public ReadOnlySpan<LemmingActionSpriteData> LemmingActionSpriteData => new(_lemmingActionSpriteData);
    public ReadOnlySpan<TribeColorData> TribeColorData => new(_tribeColorData);
}
