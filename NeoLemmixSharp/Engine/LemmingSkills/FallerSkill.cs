using static NeoLemmixSharp.Engine.LemmingSkills.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class FallerSkill : ILemmingSkill
{
    public static FallerSkill Instance { get; } = new();

    private FallerSkill()
    {
    }

    public int LemmingSkillId => 2;
    public string LemmingSkillName => "faller";

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
                lemming.CurrentSkill = WalkerSkill.Instance;
                lemming.AnimationFrame = 0;
                return;
            }
        }

        lemming.LevelPosition = pixelQueryPosition;
    }
}