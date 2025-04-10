using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FencerAction : LemmingAction, IDestructionMask
{
    public static readonly FencerAction Instance = new();

    private FencerAction()
        : base(
            EngineConstants.FencerActionId,
            EngineConstants.FencerActionName,
            EngineConstants.FencerActionSpriteFileName,
            EngineConstants.FencerAnimationFrames,
            EngineConstants.MaxFencerPhysicsFrames,
            EngineConstants.NonPermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        // Remove terrain
        var physicsFrame = lemming.PhysicsFrame;
        if (physicsFrame is >= 2 and <= 5)
        {
            TerrainMasks.ApplyFencerMask(lemming, physicsFrame - 2);
        }

        if (physicsFrame == 15)
        {
            lemming.IsStartingAction = false;
        }

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

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

            // Check whether we turn around within the next two fencer strokes (only if we don't simulate)
            if (!lemming.IsSimulation)
            {
                if (!continueWork || !lemming.IsStartingAction) // If BOTH of these are true, then both things being tested for are irrelevant
                {
                    DoFencerContinueTests(lemming, out var steelContinue, out var moveUpContinue);

                    if (!continueWork)
                    {
                        continueWork = steelContinue;
                    }

                    if (continueWork && !lemming.IsStartingAction)
                    {
                        continueWork = moveUpContinue;
                    }
                }
            }

            if (!continueWork)
            {
                if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                }
                else
                {
                    FallerAction.Instance.TransitionLemmingToAction(lemming, false);
                }
            }
        }

        if (physicsFrame is < 11 or > 14)
            return true;

        // fencer movement
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);
        bool needToUndoMoveUp;

        if (dy == 1 && physicsFrame is >= 11 and <= 13)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            dy = 0;
            needToUndoMoveUp = true;
            // This is to ignore the effect of the fencer's own slope on determining how far it can step up or down.
            // Perhaps the Fencer code should have been based off the Miner rather than the Basher...
        }
        else
        {
            needToUndoMoveUp = false;
        }

        if (dy < 0 && lemming.State.IsSlider &&
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

        if (dy < 0)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, dy);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (dy == 0)
        {
            // Move zero, one or two pixels down, if there is no steel
            if (FencerIndestructibleCheck(in gadgetsNearLemming, lemming, lemmingPosition))
            {
                FencerTurn(lemming, needToUndoMoveUp, PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 4)));
            }

            return true;
        }

        if (dy is 1 or 2)
        {
            // Move one or two pixels up, if there is no steel and not too much terrain
            if (FencerIndestructibleCheck(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, dy)))
            {
                FencerTurn(lemming, needToUndoMoveUp, PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.MoveDown(lemmingPosition, dy - 4)));

                return true;
            }

            if (!BasherAction.StepUpCheck(in gadgetsNearLemming, lemming, lemmingPosition, orientation, dx, dy))
            {
                if (FencerIndestructibleCheck(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, -2)))
                {
                    var steelTest = PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, dy)) ||
                                    PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, dy + 1));

                    FencerTurn(lemming, needToUndoMoveUp, steelTest);

                    return true;
                }

                //Stall fencer
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                if (needToUndoMoveUp)
                {
                    lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                }

                return true;
            }

            // Lem may move up
            lemmingPosition = orientation.MoveUp(lemmingPosition, dy);
            return true;
        }

        // Either stall or turn if there is steel
        if (FencerIndestructibleCheck(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            var steelTest = PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 3)) ||
                            PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 4)) ||
                            PositionIsSteelToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 5));

            FencerTurn(lemming, needToUndoMoveUp, steelTest);
            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        return true;
    }

    private static bool FencerIndestructibleCheck(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming,
        Point pos)
    {
        // Check for indestructible terrain 3 pixels above position
        return PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, Instance, lemming.Orientation.MoveUp(pos, 3));
    }

    private static void FencerTurn(
        Lemming lemming,
        bool needToUndoUp,
        bool playSound)
    {
        // Turns fencer around and transitions to walker

        var dx = -lemming.FacingDirection.DeltaX;
        var dy = needToUndoUp
            ? -1
            : 0;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition = lemming.Orientation.Move(lemmingPosition, dx, dy);

        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

        if (playSound)
        {

        }
    }

    /*
     // Simulate the behavior of the fencer in the next two frames
procedure DoFencerContinueTests(L: TLemming; var SteelContinue: Boolean; var MoveUpContinue: Boolean);
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

       SteelContinue := False;
       MoveUpContinue := False;

       // Simulate two fencer cycles
       // 11 iterations is hopefully correct: CopyL.LemPhysicsFrame changes as follows:
       // 10 -> 11 -> 12 -> 13 -> 14 -> 15 -> 16 -> 11 -> 12 -> 13 -> 14 -> 15
       CopyL.LemPhysicsFrame := 10;

       for i := 0 to 10 do
       begin
         // On CopyL.LemPhysicsFrame = 0, apply all fencer masks and jump to frame 10 again
         if (CopyL.LemPhysicsFrame = 0) then
         begin
           Inc(fSimulationDepth); // do not apply the changes to the TerrainLayer
           ApplyFencerMask(CopyL, 0);
           ApplyFencerMask(CopyL, 1);
           ApplyFencerMask(CopyL, 2);
           ApplyFencerMask(CopyL, 3);
           Dec(fSimulationDepth); // should not matter, because we do this in SimulateLem anyway, but to be safe...
           // Do *not* check whether continue fencing, but move directly ahead to frame 10
           CopyL.LemPhysicsFrame := 10;
         end;

         // Move one frame forward
         SimulateLem(CopyL, False);

         // Check if we've moved upwards
         if (CopyL.LemY < L.LemY) then
           MoveUpContinue := true;

         // Check if we have turned around at steel
         if (CopyL.LemDX = -L.LemDX) then
         begin
           SteelContinue := True;
           Break;
         end

         // Check if we are still a fencer
         else if CopyL.LemRemoved or not (CopyL.LemAction = baFencing) then
           Break; // and return false
       end;

       // Copy PhysicsMap back
       PhysicsMap.Assign(SavePhysicsMap);
       SavePhysicsMap.Free;

       // Free the copy lemming! This was missing in Nepster's code.
       CopyL.Free;
     end;
    */
    private void DoFencerContinueTests(Lemming lemming, out bool steelContinue, out bool moveUpContinue)
    {
        steelContinue = false;
        moveUpContinue = false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    string IDestructionMask.Name => LemmingActionName;

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var pixelTypeInt = (uint)pixelType;
        var orientationArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset |
                                    orientation.RotNum;
        if (((pixelTypeInt >>> orientationArrowShift) & 1U) != 0U)
            return false;

        var oppositeFacingDirectionArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset |
                                                (1 + orientation.RotNum + (facingDirection.Id << 1));
        return ((pixelTypeInt >>> oppositeFacingDirectionArrowShift) & 1U) == 0U;
    }
}