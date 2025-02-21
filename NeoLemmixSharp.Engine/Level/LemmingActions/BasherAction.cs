using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BasherAction : LemmingAction, IDestructionMask
{
    private const int BasherMaskWidth = 16;
    private const int BasherMaskHeight = 10;

    public static readonly BasherAction Instance = new();

    private static readonly PixelType[] SimulationScratchSpace = new PixelType[BasherMaskWidth * BasherMaskHeight];

    private BasherAction()
        : base(
            EngineConstants.BasherActionId,
            EngineConstants.BasherActionName,
            EngineConstants.BasherActionSpriteFileName,
            EngineConstants.BasherAnimationFrames,
            EngineConstants.MaxBasherPhysicsFrames,
            EngineConstants.NonPermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        // Remove terrain
        var physicsFrame = lemming.PhysicsFrame;
        if (physicsFrame is >= 2 and <= 5)
        {
            TerrainMasks.ApplyBasherMask(lemming, physicsFrame - 2);
        }

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        // Check for enough terrain to continue working
        if (physicsFrame == 5)
        {
            var continueWork = false;

            var fiveAbove = orientation.MoveUp(lemmingPosition, 5);
            var sixAbove = orientation.MoveUp(lemmingPosition, 6);

            // Check for destructible terrain at height 5 and 6
            for (var n = 1; n < 15; n++)
            {
                fiveAbove = orientation.MoveRight(fiveAbove, dx);
                sixAbove = orientation.MoveRight(sixAbove, dx);

                continueWork = continueWork ||
                               (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, fiveAbove) &&
                                !PositionIsIndestructibleToLemming(in gadgetsNearLemming, lemming, this, fiveAbove)) ||
                               (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, sixAbove) &&
                                !PositionIsIndestructibleToLemming(in gadgetsNearLemming, lemming, this, sixAbove));
            }

            // Check whether we turn around within the next two basher strokes (only if we don't simulate)
            if (!continueWork && !lemming.IsSimulation)
            {
                continueWork = DoTurnAtSteel(lemming);
            }

            if (continueWork)
                return true;

            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
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
        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (dy < 0 &&
            lemming.State.IsSlider &&
            DehoisterAction.LemmingCanDehoist(lemming, true, in gadgetsNearLemming))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);

            return true;
        }

        if (dy == -4)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (dy == -3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 3);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var testPoint = orientation.MoveUp(lemmingPosition, dy);
        if (dy is <= 0 and >= -2)
        {
            // Move zero, one or two pixels down, if there is no steel
            if (BasherIndestructibleCheck(in gadgetsNearLemming, lemming, testPoint))
            {
                BasherTurn(lemming, PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(testPoint, 4)));
            }
            else
            {
                lemmingPosition = testPoint;
            }

            return true;
        }

        if (dy is 1 or 2)
        {
            // Move one or two pixels up, if there is no steel and not too much terrain
            if (BasherIndestructibleCheck(in gadgetsNearLemming, lemming, testPoint))
            {
                BasherTurn(lemming, PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(testPoint, 4)));
                return true;
            }

            // Lemming may move up
            if (StepUpCheck(in gadgetsNearLemming, lemming, lemmingPosition, orientation, dx, dy))
            {
                lemmingPosition = testPoint;
                return true;
            }

            if (BasherIndestructibleCheck(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, -2)))
            {
                var steelTest = PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, dy)) ||
                                PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, dy - 1));

                BasherTurn(lemming, steelTest);
                return true;
            }

            // Stall basher
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            return true;
        }

        if (dy <= 2)
            return true;

        // Either stall or turn if there is steel
        if (BasherIndestructibleCheck(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            var steelTest = PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 3)) ||
                            PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 4)) ||
                            PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 5));

            BasherTurn(lemming, steelTest);
            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);

        return true;
    }

    private static bool BasherIndestructibleCheck(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming,
        LevelPosition pos)
    {
        var orientation = lemming.Orientation;

        // Check for indestructible terrain 3, 4 and 5 pixels above position
        return PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, Instance, orientation.MoveUp(pos, 3)) ||
               PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, Instance, orientation.MoveUp(pos, 4)) ||
               PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, Instance, orientation.MoveUp(pos, 5));
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
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming,
        LevelPosition pos,
        Orientation orientation,
        int dx,
        int dy)
    {
        var workPos = orientation.Move(pos, dx, 1);
        var p1X1Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);
        workPos = orientation.MoveUp(workPos, 1);
        var p1X2Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);
        workPos = orientation.MoveUp(workPos, 1);
        var p1X3Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);

        workPos = orientation.Move(pos, dx * 2, 1);
        var p2X1Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);
        workPos = orientation.MoveUp(workPos, 1);
        var p2X2Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);
        workPos = orientation.MoveUp(workPos, 1);
        var p2X3Y = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, workPos);

        if (dy == -1)
        {
            if (!p1X2Y && p1X1Y && p2X1Y && p2X2Y && p2X3Y)
                return false;

            if (!p1X3Y && p1X1Y && p1X2Y && p2X2Y && p2X3Y)
                return false;

            if (p1X1Y && p1X2Y && p1X3Y)
                return false;

            return true;
        }

        if (dy == -2)
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

    string IDestructionMask.Name => LemmingActionName;

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset +
                                 ((2 + orientation.RotNum - facingDirection.DeltaX) & 3);

        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}