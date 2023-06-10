using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DiggerAction : LemmingAction
{
    public const int NumberOfDiggerAnimationFrames = 16;

    public static DiggerAction Instance { get; } = new();

    private DiggerAction()
    {
    }

    public override int ActionId => 7;
    public override string LemmingActionName => "digger";
    public override int NumberOfAnimationFrames => NumberOfDiggerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(lemming, lemmingPosition, lemming.Orientation);
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.AnimationFrame--;
        }

        if (lemming.AnimationFrame >= 0 &&
            lemming.AnimationFrame <= 8)
        {
            var continueWork = DigOneRow(lemming, lemmingPosition, lemming.Orientation);
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemmingPosition, 1);

            /*if HasIndestructibleAt(L.LemX, L.LemY, L.LemDX, baDigging) then
                begin
            if HasSteelAt(L.LemX, L.LemY) then
            CueSoundEffect(SFX_HITS_STEEL, L.Position);
            Transition(L, baWalking);
            end

            else if not ContinueWork then
            Transition(L, baFalling);*/
        }

        return true;
    }

    private static bool DigOneRow(
        Lemming lemming,
        in LevelPosition baseLevelPosition,
        Orientation orientation)
    {
        var result = false;

        // Two most extreme pixels
        var checkLevelPosition = orientation.Move(baseLevelPosition, -4, 0);
        var pixel = Terrain.GetPixelData(checkLevelPosition);
        if (pixel.IsSolidToLemming(lemming))
        {
            Terrain.ErasePixel(checkLevelPosition);
        }

        checkLevelPosition = orientation.Move(baseLevelPosition, 4, 0);
        pixel = Terrain.GetPixelData(checkLevelPosition);
        if (pixel.IsSolidToLemming(lemming))
        {
            Terrain.ErasePixel(checkLevelPosition);
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.Move(baseLevelPosition, i, 0);
            pixel = Terrain.GetPixelData(checkLevelPosition);
            if (pixel.IsSolidToLemming(lemming))
            {
                Terrain.ErasePixel(checkLevelPosition);
                result = true;
            }
        }

        // Delete these pixels from the terrain layer
        // ?? if not IsSimulating then fRenderInterface.RemoveTerrain(PosX - 4, PosY, 9, 1); ??
        return result;
    }
}