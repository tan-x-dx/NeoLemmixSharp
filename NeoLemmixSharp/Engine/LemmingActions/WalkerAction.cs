using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class WalkerAction : ILemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "walker";
    public int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is WalkerAction;
    public override bool Equals(object? obj) => obj is WalkerAction;
    public override int GetHashCode() => nameof(WalkerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;

        var deltaX = lemming.FacingDirection.DeltaX(WalkerStep);
        var pixelQueryPosition = lemming.Orientation.MoveRight(originalPosition, deltaX);
        var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(ref pixelQueryPosition);

        if (pixel.IsSolid) // Check pixels going up
        {
            var i = 0;
            while (i < AscenderJump) // Simple step up
            {
                var previousQueryPosition = pixelQueryPosition;
                pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
                pixel = LevelScreen.CurrentLevel.Terrain.GetPixelData(ref pixelQueryPosition);

                if (!pixel.IsSolid)
                {
                    lemming.LevelPosition = previousQueryPosition;
                    return;
                }

                i++;
            }

            while (i < MinimumWallHeight) // Ascender step up
            {
                pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
                pixel = LevelScreen.CurrentLevel.Terrain.GetPixelData(ref pixelQueryPosition);

                if (!pixel.IsSolid)
                {
                    CommonMethods.TransitionToNewAction(lemming, AscenderAction.Instance, false);
                    /*lemming.CurrentAction = AscenderAction.Instance;
                    lemming.AnimationFrame = -1;
                    lemming.AscenderProgress = AscenderStep;*/
                    lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, AscenderStep));
                    return;
                }

                i++;
            }

            // Hit a wall! Turn around!
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }
        else // Check pixels going down
        {
            var i = 0;
            while (i < FallDistanceFall)
            {
                pixelQueryPosition = lemming.Orientation.MoveDown(pixelQueryPosition, 1);
                pixel = LevelScreen.CurrentLevel.Terrain.GetPixelData(ref pixelQueryPosition);

                if (pixel.IsSolid)
                {
                    lemming.LevelPosition = pixelQueryPosition;
                    return;
                }

                i++;
            }

            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
            /*lemming.CurrentAction = FallerAction.Instance;
            lemming.AnimationFrame = -1;*/
            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, -FallDistanceFall));
        }
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}