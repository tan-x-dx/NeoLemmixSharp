using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    public CountUpLevelTimer()
    {
        Chars[0] = ' ';
        Chars[1] = '0';
        Chars[2] = '0';
        Chars[4] = '0';
        Chars[5] = '0';

        FontColor = PanelFont.Green;
    }

    protected override void UpdateAppearance()
    {
        UpdateCountUpString(ElapsedSeconds, true);
    }
}