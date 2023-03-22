using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FallerAction : ILemmingAction
{
    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    public int LemmingActionId => 2;
    public string LemmingActionName => "faller";
    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    public void UpdateLemming(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;
        var pixelQueryPosition = originalPosition;

        for (var i = 0; i < FallerStep; i++)
        {
            pixelQueryPosition = lemming.Orientation.MoveDown(pixelQueryPosition, 1);
            var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(pixelQueryPosition);
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
}