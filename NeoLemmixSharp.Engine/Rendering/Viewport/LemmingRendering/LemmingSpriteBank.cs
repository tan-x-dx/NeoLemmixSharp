using NeoLemmixSharp.Common;
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

    public LemmingActionSprite GetActionSprite(
        LemmingAction lemmingAction,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        if (lemmingAction == NoneAction.Instance)
            return LemmingActionSprite.Empty;

        var key = GetKey(lemmingAction, orientation, facingDirection);

        return _actionSprites[key];
    }

    public static int GetKey(
        LemmingAction lemmingAction,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        if (lemmingAction == NoneAction.Instance)
            throw new InvalidOperationException("Cannot render \"None\" action");

        var lowerBits = GetKey(orientation, facingDirection);

        return (lemmingAction.Id << 3) | lowerBits;
    }

    public static int GetKey(
        Orientation orientation,
        FacingDirection facingDirection)
    {
        return DihedralTransformation.Encode(orientation, facingDirection);
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<LemmingActionSprite>(_actionSprites));
    }
}