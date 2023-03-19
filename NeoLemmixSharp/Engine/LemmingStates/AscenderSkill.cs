using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public sealed class AscenderSkill : ILemmingSkill
{
    public static AscenderSkill Instance { get; } = new();

    private AscenderSkill()
    {
    }

    public int LemmingStateId => 3;
    public string LemmingStateName => "ascender";

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
            lemming.CurrentSkill = WalkerSkill.Instance;
            lemming.AnimationFrame = 0;

            return;
        }

        lemming.LevelPosition = lemming.Orientation.MoveUp(originalPosition, AscenderStep);
        lemming.AscenderProgress += AscenderStep;
    }
}