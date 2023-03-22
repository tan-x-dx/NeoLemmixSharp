using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class AscenderAction : ILemmingAction
{
    public static AscenderAction Instance { get; } = new();

    private AscenderAction()
    {
    }

    public int LemmingActionId => 3;
    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    public string LemmingActionName => "ascender";

    public void UpdateLemming(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;
        var pixelQueryPosition = originalPosition;
        var limit = MinimumWallHeight - lemming.AscenderProgress;
        var numberOfSolidPixelsAboveCurrentPosition = 0;

        for (var i = 0; i < limit; i++)
        {
            pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
            var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(pixelQueryPosition);
            if (pixel.IsSolid)
            {
                numberOfSolidPixelsAboveCurrentPosition++;
            }
            else
            {
                break;
            }
        }

        if (numberOfSolidPixelsAboveCurrentPosition <= AscenderStep)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(pixelQueryPosition, 1);
            lemming.AscenderProgress = 0;
            lemming.CurrentAction = WalkerAction.Instance;
            lemming.AnimationFrame = -1;

            return;
        }

        lemming.LevelPosition = lemming.Orientation.MoveUp(originalPosition, AscenderStep);
        lemming.AscenderProgress += AscenderStep;
    }
}