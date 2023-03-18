using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public sealed class WalkerState : ILemmingState
{
    public static WalkerState Instance { get; } = new();

    private WalkerState()
    {
    }

    public int LemmingStateId => 1;

    public void UpdateLemming(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;

        var deltaX = lemming.FacingDirection.DeltaX(WalkerStep);
        var pixelQueryPosition = lemming.Orientation.MoveRight(originalPosition, deltaX);
        var pixel = ILemmingState.Terrain.GetPixelData(pixelQueryPosition);

        if (pixel.IsSolid) // Check pixels going up
        {
            var i = 0;
            while (i < AscenderJump) // Simple step up
            {
                var candidate = pixelQueryPosition;
                pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
                pixel = ILemmingState.Terrain.GetPixelData(pixelQueryPosition);

                if (!pixel.IsSolid)
                {
                    lemming.LevelPosition = candidate;
                    return;
                }

                i++;
            }

            while (i < MinimumWallHeight) // Ascender step up
            {
                pixelQueryPosition = lemming.Orientation.MoveUp(pixelQueryPosition, 1);
                pixel = ILemmingState.Terrain.GetPixelData(pixelQueryPosition);

                if (!pixel.IsSolid)
                {
                    lemming.CurrentState = AscenderState.Instance;
                    lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, -AscenderStep));
                    return;
                }

                i++;
            }

            // Hit a wall! Turn around!
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }
        else // Check pixels going down
        {
            var i = 0;
            while (i < FallDistanceFall)
            {
                pixelQueryPosition = lemming.Orientation.MoveDown(pixelQueryPosition, 1);
                pixel = ILemmingState.Terrain.GetPixelData(pixelQueryPosition);

                if (pixel.IsSolid)
                {
                    lemming.LevelPosition = pixelQueryPosition;
                    return;
                }

                i++;
            }

            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, FallDistanceFall));
            lemming.CurrentState = FallerState.Instance;
        }
    }



    /*
    var oldX = lemming.X;

    lemming.LevelPosition = lemming.FacingDirection.MoveInDirection(lemming.Orientation, lemming.LevelPosition, WalkerStep);
  //  lemming.X += lemming.FacingDirection.DeltaX(WalkerStep);
    var free = CommonMethods.NumberOfNonSolidPixelsBelow(lemming, FallDistanceFall);
    if (free >= FallDistanceFall)
    {
        lemming.Y += FallerStep;
    }
    else
    {
        lemming.Y += free;
        //  counter = free;
        //					if (free == 0)
        //						counter = 0; // reset fall counter
    }

    int levitation = CommonMethods.AboveGround(lemming);
    // check for flip direction
    if (levitation < WalkerObstacleHeight)
    {
        //y -= levitation;
        if (levitation >= JumperJump)
        {
            lemming.Y -= AscenderStep;
            lemming.CurrentState = AscenderState.Instance;
            return;
        }

        lemming.Y -= levitation;
    }
    else
    {
        lemming.X = oldX;
        //y = oldY;
     if (canClimb)
         {
             newType = Type.CLIMBER;
             break;
         }
         else
         {
    lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
            //}
        }
        if (free > 0)
        {
            // check for conversion to faller
            //      counter += FALLER_STEP; // @check: is this ok? increasing counter, but using free???
            if (free >= FallDistanceFall)
            {
                lemming.CurrentState = FallerState.Instance;
            }
        }
    }*/

    /*
     void Foo()
     {
         if (explode)
         {
             newType = Type.BOMBER;
             if (!nuke)
                 GameController.sound.play(GameController.SND_OHNO);
             break;
         }
         // check collision with stopper
         if (turnedByStopper())
             break;
         if (dir == Direction.RIGHT)
             x += WALKER_STEP;
         else if (dir == Direction.LEFT)
             x -= WALKER_STEP;
         // check
         free = freeBelow(FALL_DISTANCE_FALL);
         if (free >= FALL_DISTANCE_FALL)
             y += FALLER_STEP;
         else
         {
             y += free;
             counter = free;
             //					if (free == 0)
             //						counter = 0; // reset fall counter
         }
         int levitation = aboveGround();
         // check for flip direction
         if (levitation < WALKER_OBSTACLE_HEIGHT && (y + lemRes.height / 2) > 0)
         {
             //y -= levitation;
             if (levitation >= JUMPER_JUMP)
             {
                 y -= JUMPER_STEP;
                 newType = Type.JUMPER;
                 break;
             }
             else y -= levitation;
         }
         else
         {
             x = oldX;
             //y = oldY;
             if (canClimb)
             {
                 newType = Type.CLIMBER;
                 break;
             }
             else
             {
                 dir = (dir == Direction.RIGHT) ? Direction.LEFT : Direction.RIGHT;
             }
         }
         if (free > 0)
         {
             // check for conversion to faller
             counter += FALLER_STEP; // @check: is this ok? increasing counter, but using free???
             if (free >= FALL_DISTANCE_FALL)
                 newType = Type.FALLER;
         }
         break;
     }
    */
}