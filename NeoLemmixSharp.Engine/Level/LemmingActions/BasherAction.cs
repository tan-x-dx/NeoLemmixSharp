using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BasherAction : LemmingAction, IDestructionMask
{
    private const int BasherMaskWidth = 16;
    private const int BasherMaskHeight = 10;

    public static readonly BasherAction Instance = new();

    private readonly PixelType[] _simulationScratchSpace = new PixelType[BasherMaskWidth * BasherMaskHeight];

    private BasherAction()
        : base(
            LevelConstants.BasherActionId,
            LevelConstants.BasherActionName,
            LevelConstants.BasherAnimationFrames,
            LevelConstants.MaxBasherPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        // Remove terrain
        var physicsFrame = lemming.PhysicsFrame;
        if (physicsFrame is >= 2 and <= 5)
        {
            TerrainMasks.ApplyBasherMask(lemming, physicsFrame - 2);
        }

        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        // Check for enough terrain to continue working
        if (physicsFrame == 5)
        {
            var continueWork = false;

            var fiveAbove = orientation.Move(lemmingPosition, 0, 5);
            var sixAbove = orientation.Move(lemmingPosition, 0, 6);

            // Check for destructible terrain at height 5 and 6
            for (var n = 1; n < 15; n++)
            {
                fiveAbove = orientation.MoveRight(fiveAbove, dx);
                sixAbove = orientation.MoveRight(sixAbove, dx);

                continueWork = continueWork ||
                               (terrainManager.PixelIsSolidToLemming(lemming, fiveAbove) &&
                                !terrainManager.PixelIsIndestructibleToLemming(lemming, this, fiveAbove)) ||
                               (terrainManager.PixelIsSolidToLemming(lemming, sixAbove) &&
                                !terrainManager.PixelIsIndestructibleToLemming(lemming, this, sixAbove));
            }

            // Check whether we turn around within the next two basher strokes (only if we don't simulate)
            if (!continueWork && !lemming.IsSimulation)
            {
                continueWork = DoTurnAtSteel(lemming);
            }

            if (continueWork)
                return true;

            if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            }

            return true;
        }

        if (physicsFrame is < 11 or > 15)
            return true;
        
        // Basher movement
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        var dy = FindGroundPixel(lemming, lemmingPosition);

        if (dy > 0 &&
            lemming.State.IsSlider &&
            DehoisterAction.LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);

            return true;
        }

        if (dy == 4)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 4);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (dy == 3)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 3);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var testPoint = orientation.MoveDown(lemmingPosition, dy);
        if (dy is >= 0 and <= 2)
        {
            // Move zero, one or two pixels down, if there is no steel
            if (BasherIndestructibleCheck(lemming, testPoint))
            {
                BasherTurn(lemming, terrainManager.PixelIsSteel(orientation.MoveUp(testPoint, 4)));
            }
            else
            {
                lemmingPosition = testPoint;
            }

            return true;
        }

        if (dy is -1 or -2)
        {
            // Move one or two pixels up, if there is no steel and not too much terrain
            if (BasherIndestructibleCheck(lemming, testPoint))
            {
                BasherTurn(lemming, terrainManager.PixelIsSteel(orientation.MoveUp(testPoint, 4)));
                return true;
            }

            if (!StepUpCheck(lemming, lemmingPosition, orientation, dx, dy))
            {
                if (BasherIndestructibleCheck(lemming, orientation.Move(lemmingPosition, dx, 0 - 2)))
                {
                    var steelTest = terrainManager.PixelIsSteel(orientation.Move(lemmingPosition, dx, -dy)) ||
                                    terrainManager.PixelIsSteel(orientation.Move(lemmingPosition, dx, -dy - 1));

                    BasherTurn(lemming, steelTest);
                    return true;
                }

                // Stall basher
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                return true;
            }

            // Lemming may move up
            lemmingPosition = testPoint;
            return true;
        }

        if (dy >= -2)
            return true;

        // Either stall or turn if there is steel
        if (BasherIndestructibleCheck(lemming, lemmingPosition))
        {
            var steelTest = terrainManager.PixelIsSteel(orientation.MoveUp(lemmingPosition, 3)) ||
                            terrainManager.PixelIsSteel(orientation.MoveUp(lemmingPosition, 4)) ||
                            terrainManager.PixelIsSteel(orientation.MoveUp(lemmingPosition, 5));

            BasherTurn(lemming, steelTest);
            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);

        return true;
    }

    private static bool BasherIndestructibleCheck(
        Lemming lemming,
        LevelPosition pos)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;

        // Check for indestructible terrain 3, 4 and 5 pixels above position
        return terrainManager.PixelIsIndestructibleToLemming(lemming, Instance, orientation.MoveUp(pos, 3)) ||
               terrainManager.PixelIsIndestructibleToLemming(lemming, Instance, orientation.MoveUp(pos, 4)) ||
               terrainManager.PixelIsIndestructibleToLemming(lemming, Instance, orientation.MoveUp(pos, 5));
    }

    private static void BasherTurn(
        Lemming lemming,
        bool playSound)
    {
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.LevelPosition;
        lemmingPosition = lemming.Orientation.MoveLeft(lemmingPosition, dx);
        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

        if (playSound)
        {

        }
    }

    public static bool StepUpCheck(
        Lemming lemming,
        LevelPosition pos,
        Orientation orientation,
        int dx,
        int dy)
    {
        var terrainManager = LevelScreen.TerrainManager;

        var p1X1Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx, 1));
        var p1X2Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx, 2));
        var p1X3Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx, 3));

        var p2X1Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx * 2, 1));
        var p2X2Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx * 2, 2));
        var p2X3Y = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(pos, dx * 2, 3));

        if (dy == 1)
        {
            if (!p1X2Y && p1X1Y && p2X1Y && p2X2Y && p2X3Y)
                return false;

            if (!p1X3Y && p1X1Y && p1X2Y && p2X2Y && p2X3Y)
                return false;

            if (p1X1Y && p1X2Y && p1X3Y)
                return false;

            return true;
        }

        if (dy == 2)
        {
            if (!p1X2Y && p1X1Y && p2X1Y && p2X2Y && p2X3Y)
                return false;

            if (!p1X3Y && p1X2Y && p2X2Y && p2X3Y)
                return false;

            if (p1X2Y && p1X3Y)
                return false;
        }

        return true;
    }

    /*
    function DoTurnAtSteel(L: TLemming): Boolean;
    var
    CopyL: TLemming;
    SavePhysicsMap: TBitmap32;
    i: Integer;
    begin
    // Make deep copy of the lemming
    CopyL := TLemming.Create;
    CopyL.Assign(L);
    CopyL.LemIsPhysicsSimulation := true;

    // Make a deep copy of the PhysicsMap
    SavePhysicsMap := TBitmap32.Create;
    SavePhysicsMap.Assign(PhysicsMap);

    Result := False;

    // Simulate two basher cycles
    // 11 iterations is hopefully correct: CopyL.LemPhysicsFrame changes as follows:
    // 10 -> 11 -> 12 -> 13 -> 14 -> 15 -> 16 -> 11 -> 12 -> 13 -> 14 -> 15
    CopyL.LemPhysicsFrame := 10;
    for i := 0 to 10 do
    begin
      // On CopyL.LemPhysicsFrame = 0 or 16, apply all basher masks and jump to frame 10 again
      if (CopyL.LemPhysicsFrame in [0, 16]) then
      begin
        Inc(fSimulationDepth);
        ApplyBashingMask(CopyL, 0);
        ApplyBashingMask(CopyL, 1);
        ApplyBashingMask(CopyL, 2);
        ApplyBashingMask(CopyL, 3);
        Dec(fSimulationDepth); // should not matter, because we do this in SimulateLem anyway, but to be safe...
        // Do *not* check whether continue bashing, but move directly ahead to frame 10
        CopyL.LemPhysicsFrame := 10;
      end;

      // Move one frame forward
      SimulateLem(CopyL, False);

      // Check if we have turned around at steel
      if (CopyL.LemDX = -L.LemDX) then
      begin
        Result := True;
        Break;
      end
      // Check if we are still a basher
      else if CopyL.LemRemoved or not (CopyL.LemAction = baBashing) then
        Break; // and return false
    end;

    // Copy PhysicsMap back
    PhysicsMap.Assign(SavePhysicsMap);
    SavePhysicsMap.Free;

    // Free CopyL
    CopyL.Free;
    end;
    */
    // Simulate the behavior of the basher in the next two frames
    private bool DoTurnAtSteel(Lemming lemming)
    {
        // Make deep copy of the lemming

        var lemmingManager = LevelScreen.LemmingManager;
        var simulationLemming = Lemming.SimulationLemming;
        simulationLemming.SetRawDataFromOther(lemming);

        // Make a deep copy of the PhysicsMap




        var result = false;



        // Simulate two basher cycles
        // 11 iterations is hopefully correct: CopyL.LemPhysicsFrame changes as follows:
        // 10 -> 11 -> 12 -> 13 -> 14 -> 15 -> 16 -> 11 -> 12 -> 13 -> 14 -> 15

        simulationLemming.PhysicsFrame = 10;
        for (var i = 0; i < 11; i++)
        {
            // When simulation lemming's physicsFrame is 0 or 16, apply all basher masks and jump to frame 10 again
            if (simulationLemming.PhysicsFrame is >= 0 and <= 16)
            {



                simulationLemming.PhysicsFrame = 10;
            }
            else
            {
                ;
            }

            simulationLemming.Simulate(false);


            if (simulationLemming.FacingDirection != lemming.FacingDirection)
            {
                result = true;
                break;
            }

            // Check if we are still a basher
            if (!simulationLemming.State.IsActive || simulationLemming.CurrentAction != Instance)
            {
                break; // and return false
            }
        }

        return result;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var bashDirectionAsOrientation = facingDirection.ConvertToRelativeOrientation(orientation);
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                 Orientation.GetOpposite(bashDirectionAsOrientation).RotNum;
        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}