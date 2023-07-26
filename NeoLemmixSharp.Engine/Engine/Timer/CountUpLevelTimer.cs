using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    private static readonly Color MainColor = new(0x00, 0xB0, 0x00);

    public CountUpLevelTimer()
    {
        Chars[0] = ' ';
        Chars[1] = '0';
        Chars[2] = '0';
        Chars[4] = '0';
        Chars[5] = '0';

        FontColor = MainColor;
    }

    public override void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % GameConstants.FramesPerSecond != 0)
            return;

        ElapsedSeconds++;
        UpdateCountUpString(ElapsedSeconds);
    }

    protected override void UpdateAppearance()
    {
        UpdateCountUpString(ElapsedSeconds);
    }
}