using static NeoLemmixSharp.Engine.LemmingSkills.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class AscenderSkill : ILemmingSkill
{
    public static AscenderSkill Instance { get; } = new();

    private AscenderSkill()
    {
    }

    public int LemmingSkillId => 3;
    public string LemmingSkillName => "ascender";

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