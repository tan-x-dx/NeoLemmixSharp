using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class WalkerAction : ILemmingAction
{
    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public int LemmingActionId => 1;
    public string LemmingActionName => "walker";
    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

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
                var candidate = pixelQueryPosition;
                pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
                pixel = LevelScreen.CurrentLevel.Terrain.GetPixelData(ref pixelQueryPosition);

                if (!pixel.IsSolid)
                {
                    lemming.LevelPosition = candidate;
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
                    lemming.CurrentAction = AscenderAction.Instance;
                    lemming.AnimationFrame = -1;
                    lemming.AscenderProgress = AscenderStep;
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

            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, -FallDistanceFall));
            lemming.CurrentAction = FallerAction.Instance;
            lemming.AnimationFrame = -1;
        }
    }
}