﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ExploderAction : LemmingAction, IDestructionAction
{
    public const int NumberOfExploderAnimationFrames = 1;

    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public override int Id => 26;
    public override string LemmingActionName => "bomber";
    public override int NumberOfAnimationFrames => NumberOfExploderAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        // Bombers do not care about arrows, only if the pixel can be destroyed at all!
        // Since other checks have already taken place,
        // this code is only ever reached when the pixel can be destroyed by a bomber

        return true;
    }
}