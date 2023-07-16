using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class DiggerAction : LemmingAction
{
    public const int NumberOfDiggerAnimationFrames = 16;

    public static DiggerAction Instance { get; } = new();

    private DiggerAction()
    {
    }

    public override int Id => 7;
    public override string LemmingActionName => "digger";
    public override int NumberOfAnimationFrames => NumberOfDiggerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(lemming, orientation, orientation.MoveUp(lemmingPosition, 1));
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.AnimationFrame--;
        }

        if (lemming.AnimationFrame < 0 ||
            lemming.AnimationFrame > 8)
            return true;

        var continueDigging = DigOneRow(lemming, orientation, lemmingPosition);

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        if (Terrain.PixelIsIndestructibleToLemming(lemmingPosition, lemming))
        {
            // if HasSteelAt(L.LemX, L.LemY) then
            //    CueSoundEffect(SFX_HITS_STEEL, L.Position);

            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (continueDigging)
            return true;

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    private static bool DigOneRow(Lemming lemming, Orientation orientation, LevelPosition lemmingPosition)
    {
        // The central pixel of the removed row lies at the lemming's position
        var result = false;

        // Two most extreme pixels
        var checkLevelPosition = orientation.Move(lemmingPosition, -4, 0);
        var pixelIsSolid = Terrain.PixelIsSolidToLemming(checkLevelPosition, lemming);
        if (pixelIsSolid)
        {
            Terrain.ErasePixel(checkLevelPosition);
        }

        checkLevelPosition = orientation.Move(lemmingPosition, 4, 0);
        pixelIsSolid = Terrain.PixelIsSolidToLemming(checkLevelPosition, lemming);
        if (pixelIsSolid)
        {
            Terrain.ErasePixel(checkLevelPosition);
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.Move(lemmingPosition, i, 0);
            pixelIsSolid = Terrain.PixelIsSolidToLemming(checkLevelPosition, lemming);
            if (pixelIsSolid)
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
