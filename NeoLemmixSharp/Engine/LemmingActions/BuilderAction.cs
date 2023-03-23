using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class BuilderAction : ILemmingAction
{
    public const int NumberOfBuilderAnimationFrames = 16;

    public static BuilderAction Instance { get; } = new();

    private BuilderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "builder";
    public int NumberOfAnimationFrames => NumberOfBuilderAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is BuilderAction;
    public override bool Equals(object? obj) => obj is BuilderAction;
    public override int GetHashCode() => nameof(BuilderAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.NumberOfBricksLeft == 0)
        {
            ;
        }

        if (lemming.AnimationFrame == 9)
        {
            CommonMethods.LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 && lemming.NumberOfBricksLeft <= 3)
        {
            // play sound/make visual cue
        }
        else if (lemming.AnimationFrame == 0)
        {
            BuilderFrame0(lemming);
            lemming.ConstructivePositionFreeze = false;
        }
    }

    private static void BuilderFrame0(Lemming lemming)
    {
        var originalPosition = lemming.LevelPosition;

        lemming.NumberOfBricksLeft--;
        var deltaX = lemming.FacingDirection.DeltaX(1);
        var checkPositionDelta = new LevelPosition(deltaX, 2);
        var pixelQueryPosition = lemming.Orientation.Move(originalPosition, checkPositionDelta);
        var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(ref pixelQueryPosition);

        if (pixel.IsSolid)
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            return;
        }

        if (lemming.NumberOfBricksLeft > 0 &&
            (PixelIsSolid(lemming, deltaX, 3) ||
             PixelIsSolid(lemming, deltaX + deltaX, 2) ||
             PixelIsSolid(lemming, deltaX + deltaX, 10)))
        {
            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(deltaX, 1));
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemming.LevelPosition = lemming.Orientation.Move(originalPosition, new LevelPosition(deltaX + deltaX, 1));
            return;
        }

        if (lemming.NumberOfBricksLeft > 0 &&
            (PixelIsSolid(lemming, 0, 2) ||
             PixelIsSolid(lemming, 0, 3) ||
             PixelIsSolid(lemming, deltaX, 3) ||
             PixelIsSolid(lemming, deltaX, 9)))
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            return;
        }

        if (lemming.NumberOfBricksLeft == 0)
        {
            CommonMethods.TransitionToNewAction(lemming, ShruggerAction.Instance, false);
        }
    }

    public void OnTransitionToAction(Lemming lemming)
    {
        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }


    /*

    begin
      Result := True;

      if L.LemPhysicsFrame = 9 then
        LayBrick(L)

      else if (L.LemPhysicsFrame = 10) and (L.LemNumberOfBricksLeft <= 3) then
        CueSoundEffect(SFX_BUILDER_WARNING, L.Position)

      else if L.LemPhysicsFrame = 0 then
      begin
        Dec(L.LemNumberOfBricksLeft);

        if HasPixelAt(L.LemX + L.LemDx, L.LemY - 2) then
          Transition(L, baWalking, True)  // turn around as well

        else if (     HasPixelAt(L.LemX + L.LemDx, L.LemY - 3)
                  or  HasPixelAt(L.LemX + 2*L.LemDx, L.LemY - 2)
                  or (HasPixelAt(L.LemX + 2*L.LemDx, L.LemY - 10) and (L.LemNumberOfBricksLeft > 0))
                ) then
        begin
          Dec(L.LemY);
          Inc(L.LemX, L.LemDx);
          Transition(L, baWalking, True)  // turn around as well
        end

        else
        begin
          if not L.LemConstructivePositionFreeze then
          begin
            Dec(L.LemY);
            Inc(L.LemX, 2*L.LemDx);
          end;

          if (     HasPixelAt(L.LemX, L.LemY - 2)
               or  HasPixelAt(L.LemX, L.LemY - 3)
               or  HasPixelAt(L.LemX + L.LemDx, L.LemY - 3)
               or (HasPixelAt(L.LemX + L.LemDx, L.LemY - 9) and (L.LemNumberOfBricksLeft > 0))
             ) then
             Transition(L, baWalking, True)  // turn around as well

          else if L.LemNumberOfBricksLeft = 0 then
             Transition(L, baShrugging);
        end;
      end;

      if L.LemPhysicsFrame = 0 then
        L.LemConstructivePositionFreeze := false;
    end;

    */
    private static bool PixelIsSolid(Lemming lemming, int dx, int dy)
    {
        var pixelQueryPosition = lemming.Orientation.Move(lemming.LevelPosition, new LevelPosition(dx, dy));
        var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(ref pixelQueryPosition);
        return pixel.IsSolid;
    }

    private static void QuitBuildingAndTurnAround(Lemming lemming)
    {
        lemming.CurrentAction = WalkerAction.Instance;
        lemming.AnimationFrame = -1;
        lemming.NumberOfBricksLeft = 0;
        lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
    }
}
