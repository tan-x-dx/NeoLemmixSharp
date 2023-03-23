using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FallerAction : ILemmingAction
{
    public const int NumberOfFallerAnimationFrames = 4;

    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "faller";
    public int NumberOfAnimationFrames => NumberOfFallerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is FallerAction;
    public override bool Equals(object? obj) => obj is FallerAction;
    public override int GetHashCode() => nameof(FallerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;
        var pixelQueryPosition = originalPosition;

        for (var i = 0; i < FallerStep; i++)
        {
            pixelQueryPosition = lemming.Orientation.MoveDown(pixelQueryPosition, 1);
            var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(ref pixelQueryPosition);
            if (pixel.IsSolid)
            {
                lemming.LevelPosition = pixelQueryPosition;
                lemming.CurrentAction = WalkerAction.Instance;
                lemming.AnimationFrame = -1;
                return;
            }
        }

        lemming.LevelPosition = pixelQueryPosition;
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}