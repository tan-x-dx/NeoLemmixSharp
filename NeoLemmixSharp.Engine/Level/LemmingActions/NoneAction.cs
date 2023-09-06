﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class NoneAction : LemmingAction
{
    /// <summary>
    /// Logically equivalent to null, but null references suck
    /// </summary>
    public static NoneAction Instance { get; } = new();

    private NoneAction()
    {
    }

    public override int Id => -1;
    public override string LemmingActionName => "none";
    public override int NumberOfAnimationFrames => 1;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => -1;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override LevelPosition GetAnchorPosition() => new(0, 0);

    protected override int TopLeftBoundsDeltaX() => 0;
    protected override int TopLeftBoundsDeltaY() => 0;

    protected override int BottomRightBoundsDeltaX() => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
    }
}