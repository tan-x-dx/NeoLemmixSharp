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

    protected override int ActionId => 7;
    public override string LemmingActionName => "digger";
    public override int NumberOfAnimationFrames => NumberOfDiggerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(lemming.LevelPosition, lemming.Orientation);
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.AnimationFrame--;
        }

        if (lemming.AnimationFrame >= 0 &&
            lemming.AnimationFrame <= 8)
        {
            var continueWork = DigOneRow(lemming.LevelPosition, lemming.Orientation);
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);

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
        LevelPosition levelPosition,
        IOrientation orientation)
    {
        var result = false;

        // Two most extreme pixels
        var pixel = Terrain.GetPixelData(orientation.Move(levelPosition, -4, 0));
        if (pixel.IsSolid)
        {
            //    Terrain.ErasePixel();
        }
        pixel = Terrain.GetPixelData(orientation.Move(levelPosition, 4, 0));
        if (pixel.IsSolid)
        {
            //    Terrain.ErasePixel();
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            pixel = Terrain.GetPixelData(orientation.Move(levelPosition, i, 0));
            if (pixel.IsSolid)
            {
                //    Terrain.ErasePixel();
                result = true;
            }
        }

        // Delete these pixels from the terrain layer
        // ?? if not IsSimulating then fRenderInterface.RemoveTerrain(PosX - 4, PosY, 9, 1); ??
        return result;
    }
}