﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ExploderAction : LemmingAction, IDestructionAction
{
    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public override int Id => GameConstants.ExploderActionId;
    public override string LemmingActionName => "bomber";
    public override int NumberOfAnimationFrames => GameConstants.ExploderAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        // Bombers do not care about arrows, only if the pixel can be destroyed at all!
        // Since other checks will have already taken place, this code is only ever
        // reached when the pixel can definitely be destroyed by a bomber.
        // Therefore, just return true.

        return true;
    }
}