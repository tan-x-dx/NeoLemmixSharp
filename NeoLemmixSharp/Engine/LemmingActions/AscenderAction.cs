using NeoLemmixSharp.Rendering;

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

    public bool Equals(ILemmingAction? other) => other is AscenderAction;
    public override bool Equals(object? obj) => obj is AscenderAction;
    public override int GetHashCode() => nameof(AscenderAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        var terrain = LevelScreen.CurrentLevel!.Terrain;
        var pixelQueryPosition = lemming.LevelPosition;
        var pixel = terrain.GetPixelData(ref pixelQueryPosition);
        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               pixel.IsSolid)
        {
            dy++;
            pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
            pixel = terrain.GetPixelData(ref pixelQueryPosition);
            lemming.LevelPosition = pixelQueryPosition;
            lemming.AscenderProgress++;
        }

        pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
        pixel = terrain.GetPixelData(ref pixelQueryPosition);
        if (dy < 2 && !pixel.IsSolid)
        {
            //??fLemNextAction := baWalking;??
        }
        else
        {
            var pixel1 = terrain.GetPixelData(lemming.LevelPosition, lemming.Orientation, 0, 1);
            var pixel2 = terrain.GetPixelData(lemming.LevelPosition, lemming.Orientation, 0, 2);

            if ((lemming.AscenderProgress == 4 &&
                 pixel1.IsSolid &&
                 pixel2.IsSolid) ||
                (lemming.AscenderProgress >= 5 &&
                 pixel1.IsSolid))
            {
                var dx = lemming.FacingDirection.DeltaX(1);
                lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
                CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, true);
            }
        }
    }

    public void OnTransitionToAction(Lemming lemming)
    {
        lemming.AscenderProgress = 0;
    }
}