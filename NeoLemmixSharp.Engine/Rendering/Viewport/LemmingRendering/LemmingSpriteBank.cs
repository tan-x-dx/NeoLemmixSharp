using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingSpriteBank : IDisposable
{
    private readonly LemmingActionSprite[] _actionSprites;
    private readonly TeamColorData[] _teamColorData;

    public LemmingSpriteBank(LemmingActionSprite[] actionSprites, TeamColorData[] teamColorData)
    {
        _actionSprites = actionSprites;
        _teamColorData = teamColorData;
    }

    public TeamColorData GetColorData(int id) => _teamColorData[id];

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