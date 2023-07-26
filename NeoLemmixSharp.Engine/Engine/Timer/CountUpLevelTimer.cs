namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    public CountUpLevelTimer()
    {
        Chars[0] = ' ';
        Chars[1] = '0';
        Chars[2] = '0';
        Chars[4] = '0';
        Chars[5] = '0';
    }

    public override void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % 2 != 0)
            return;

        ElapsedSeconds++;
        UpdateCountUpString(ElapsedSeconds);
    }
}