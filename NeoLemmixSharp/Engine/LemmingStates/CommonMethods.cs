using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public static class CommonMethods
{
    public static int FreeBelow(Lemming lemming, int step)
    {
        int free = 0;
        int pos = lemming.X;
        var level = LevelScreen.CurrentLevel!;
        var stencil = level.WrappedTerrain;
        int yb = lemming.Y + 1;
        pos = lemming.X + yb * level.Width; // line below the lemming
        for (int i = 0; i < step; i++)
        {
            if (yb + i >= level.Height)
                return FallDistanceForceFall; // convert most skill to faller
            var solid = stencil.Get(lemming.X, yb);
            if (!solid)
            {
                free++;
            }
            else
            {
                break;
            }
            pos += level.Width;
        }
        return free;
    }

    public static int AboveGround(Lemming lemming)
    {
        var level = LevelScreen.CurrentLevel!;
        if (lemming.X < 0 || lemming.X >= level.Width)
            return level.Height - 1;

        int ym = lemming.Y;
        if (ym >= level.Height)
            return level.Height - 1;

        int pos = lemming.X;
        var stencil = level.WrappedTerrain;
        pos += ym * level.Width;
        int levitation;
        for (levitation = 0; levitation < WalkerObstacleHeight; levitation++, pos -= level.Width, ym--)
        {
            if (ym < 0)
                return WalkerObstacleHeight + 1; // forbid leaving level to the top
            if (!stencil.Get(lemming.X, ym))
                break;
        }

        return levitation;
    }
}