using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class BuilderAction : ILemmingAction
{
    public const int NumberOfBuilderAnimationFrames = 16;

    public static BuilderAction Instance { get; } = new();

    private BuilderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "builder";
    public int NumberOfAnimationFrames => NumberOfBuilderAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is BuilderAction;
    public override bool Equals(object? obj) => obj is BuilderAction;
    public override int GetHashCode() => nameof(BuilderAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == 9)
        {
            CommonMethods.LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 &&
                 lemming.NumberOfBricksLeft <= 3)
        {
            // play sound/make visual cue
        }
        else if (lemming.AnimationFrame == 0)
        {
            BuilderFrame0(lemming);
            lemming.ConstructivePositionFreeze = false;
        }
    }

    private static void BuilderFrame0(Lemming lemming)
    {
        lemming.NumberOfBricksLeft--;

        var dx = lemming.FacingDirection.DeltaX;
        if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 2)).IsSolid)
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
        }
        else if (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 3)).IsSolid ||
                 Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 2)).IsSolid ||
                 (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 10)).IsSolid &&
                  lemming.NumberOfBricksLeft > 0))
        {
            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, 1);
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
        }
        else
        {
            if (!lemming.ConstructivePositionFreeze)
            {
                lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 1);
            }

            if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 2)).IsSolid ||
                Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 3)).IsSolid ||
                Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 3)).IsSolid ||
                (Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 10)).IsSolid &&
                 lemming.NumberOfBricksLeft > 0))
            {
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                CommonMethods.TransitionToNewAction(lemming, ShruggerAction.Instance, false);
            }
        }
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}
