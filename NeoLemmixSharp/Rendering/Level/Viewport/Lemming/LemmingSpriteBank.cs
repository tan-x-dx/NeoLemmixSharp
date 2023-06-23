using System;
using NeoLemmixSharp.Engine.Actions;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Orientations;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public sealed class LemmingSpriteBank : IDisposable
{
    private readonly ActionSprite[] _actionSprites;

    public LemmingSpriteBank(ActionSprite[] actionSprites)
    {
        _actionSprites = actionSprites;
    }

    public ActionSprite GetActionSprite(
        LemmingAction lemmingAction,
        Orientation orientation,
        FacingDirection facingDirection)
    {
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

        return (lemmingAction.ActionId << 3) | (orientation.RotNum << 1) | (facingDirection.FacingId);
    }
    
    public void Dispose()
    {
        for (var i = 0; i < _actionSprites.Length; i++)
        {
            _actionSprites[i].Dispose();
        }
    }
}