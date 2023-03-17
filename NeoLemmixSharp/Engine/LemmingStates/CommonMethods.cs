using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public static class CommonMethods
{
    public static int NumberOfNonSolidPixelsBelow(
        Lemming lemming,
        LevelPosition pos,
        int step)
    {
        


        return 0;
        /* int free = 0;
         int pos = lemming.X;
         var level = LevelScreen.CurrentLevel!;
         var terrain = level.Terrain;
         int yb = lemming.Y + 1;
         pos = lemming.X + yb * level.Width; // line below the lemming
         for (int i = 0; i < step; i++)
         {
             if (yb + i >= level.Height)
                 return FallDistanceForceFall; // convert most skill to faller
             var solid = terrain.GetPixelData(lemming.X, yb);
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
         return free;*/
    }

    public static int AboveGround(Lemming lemming)
    {
        return 0;
        /*   var level = LevelScreen.CurrentLevel!;
           if (lemming.X < 0 || lemming.X >= level.Width)
               return level.Height - 1;

           int ym = lemming.Y;
           if (ym >= level.Height)
               return level.Height - 1;

           int pos = lemming.X;
           var terrain = level.Terrain;
           pos += ym * level.Width;
           int levitation;
           for (levitation = 0; levitation < WalkerObstacleHeight; levitation++, pos -= level.Width, ym--)
           {
               if (ym < 0)
                   return WalkerObstacleHeight + 1; // forbid leaving level to the top
               if (!terrain.GetPixelData(lemming.X, ym))
                   break;
           }

           return levitation;*/
    }
}