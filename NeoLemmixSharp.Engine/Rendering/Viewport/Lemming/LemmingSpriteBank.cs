using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Teams;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public sealed class LemmingSpriteBank : IDisposable
{
    private readonly ActionSprite[] _actionSprites;
    private readonly TeamColorData[] _teamColorData;

    public LemmingSpriteBank(ActionSprite[] actionSprites, TeamColorData[] teamColorData)
    {
        _actionSprites = actionSprites;
        _teamColorData = teamColorData;
    }

    public ActionSprite GetActionSprite(
        LemmingAction lemmingAction,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        var key = GetKey(lemmingAction, orientation, facingDirection);

        return _actionSprites[key];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetKey(
        Orientation orientation,
        FacingDirection facingDirection)
    {
        return (orientation.RotNum << 1) | facingDirection.Id;
    }

    public void Dispose()
    {
        for (var i = 0; i < _actionSprites.Length; i++)
        {
            _actionSprites[i].Dispose();
        }
    }

    public void SetTeamColors()
    {
        for (var i = 0; i < GameConstants.NumberOfTeams; i++)
        {
            Team.AllItems[i].SetColorData(_teamColorData[i]);
        }
    }
}