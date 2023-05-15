namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ShimmierAction : LemmingAction
{
    public const int NumberOfShimmierAnimationFrames = 20;

    public static ShimmierAction Instance { get; } = new();

    private ShimmierAction()
    {
    }

    protected override int ActionId => 23;
    public override string LemmingActionName => "shimmier";
    public override int NumberOfAnimationFrames => NumberOfShimmierAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        if ((lemming.AnimationFrame & 1) == 0)
        {
            var i = 0;
            // Check whether we find terrain to walk onto
            for (; i < 3; i++)
            {
                if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, i)).IsSolid &&
                    !Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, i + 1)).IsSolid)
                {
                    lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, i);
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                    return true;
                }
            }

            // Check whether we find terrain to hoist onto
            for (; i < 6; i++)
            {
                if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, i)).IsSolid &&
                    !Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, i + 1)).IsSolid)
                {
                    lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, i - 4);
                    lemming.IsStartingAction = false;
                    HoisterAction.Instance.TransitionLemmingToAction(lemming, false);
                    lemming.AnimationFrame += 2;
                    return true;
                }
            }

            // Check whether we fall down due to a wall
            for (; i < 8; i++)
            {
                if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, i)).IsSolid)
                {
                    if (lemming.IsSlider)
                    {
                        SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                    }
                    else
                    {
                        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
                    }

                    return true;
                }
            }
        }

        var pixel9AboveIsSolid = Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 9)).IsSolid;
        // Check whether we fall down due to not enough ceiling terrain
        if (!pixel9AboveIsSolid &&
            !Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 10)).IsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Check whether we fall down due a checkerboard ceiling
        if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 8)).IsSolid &&
            !pixel9AboveIsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Move along
        lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);

        if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8)).IsSolid)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);

            if (Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            var bottomOfLevel = lemming.Orientation.BottomLeftCornerOfLevel();
            if (lemming.Orientation.FirstIsBelowSecond(lemming.LevelPosition, lemming.Orientation.MoveDown(bottomOfLevel, 8)))
            {
                // ?? RemoveLemming(L, RM_NEUTRAL); ??
                return true;
            }
        }

        if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 9)).IsSolid)
            return true;

        lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
        var pos = lemming.Orientation.MoveUp(lemming.LevelPosition, 5);
        if (Terrain.GetPixelData(pos).IsSolid)
        {
            lemming.LevelPosition = pos;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);

            if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8)).IsSolid)
            {
                lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 2);
            if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8)).IsSolid)
            {
                lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
            }
        }
        else if (lemming.CurrentAction == JumperAction.Instance)
        {
            for (var i = -1; i < 4; i++)
            {
                if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 9 + i)).IsSolid &&
                    !Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8 + i)).IsSolid)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, i);
                }
            }
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}