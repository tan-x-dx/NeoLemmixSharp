using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class AscenderAction : ILemmingAction
{
    public const int NumberOfAscenderAnimationFrames = 1;

    public static AscenderAction Instance { get; } = new();

    private AscenderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "ascender";
    public int NumberOfAnimationFrames => NumberOfAscenderAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is AscenderAction;
    public override bool Equals(object? obj) => obj is AscenderAction;
    public override int GetHashCode() => nameof(AscenderAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1)).IsSolid)
        {
            dy++;
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1 = Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1));
        var pixel2 = Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 2));

        if (dy < 2 &&
            !pixel1.IsSolid)
        {
            lemming.NextAction = WalkerAction.Instance;
        }
        else if ((lemming.AscenderProgress == 4 &&
                  pixel1.IsSolid &&
                  pixel2.IsSolid) ||
                 (lemming.AscenderProgress >= 5 &&
                  pixel1.IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, true);
        }

        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
        lemming.AscenderProgress = 0;
    }
}