using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingSpriteBank : IDisposable
{
    private readonly LemmingActionSprite[] _actionSprites;
    private readonly TribeColorData[] _tribeColorData;

    public LemmingSpriteBank(LemmingActionSprite[] actionSprites, TribeColorData[] tribeColorData)
    {
        _actionSprites = actionSprites;
        _tribeColorData = tribeColorData;
    }

    public TribeColorData GetColorData(int id) => _tribeColorData[id];

    public LemmingActionSprite GetActionSprite(LemmingAction lemmingAction)
    {
        var id = lemmingAction.Id;
        if ((uint)id < (uint)_actionSprites.Length)
            return _actionSprites[id];

        return LemmingActionSprite.Empty;
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<LemmingActionSprite>(_actionSprites));
    }
}