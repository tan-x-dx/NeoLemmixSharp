using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingSpriteBank
{
    private readonly ListLookup<StyleIdentifier, SpriteBankData> _lookupData;

    public LemmingSpriteBank(ListLookup<StyleIdentifier, SpriteBankData> lookupData)
    {
        _lookupData = lookupData;
    }

    public ref readonly TribeColorData GetColorData(TribeStyleIdentifier tribeIdentifier)
    {
        var spriteBankData = _lookupData[tribeIdentifier.StyleIdentifier];

        return ref spriteBankData.TribeColorData[tribeIdentifier.ThemeTribeId];
    }

    public LemmingActionSprite GetActionSprite(Lemming lemming)
    {
        var actionSprites = GetActionSprites(lemming);

        var id = lemming.CurrentAction.Id;
        if ((uint)id < (uint)actionSprites.Length)
            return actionSprites[id];

        return LemmingActionSprite.Empty;
    }

    private LemmingActionSprite[] GetActionSprites(Lemming lemming)
    {
        var styleIdentifier = lemming.State.TribeAffiliation.TribeIdentifier.StyleIdentifier;
        return _lookupData[styleIdentifier].ActionSprites;
    }
}
