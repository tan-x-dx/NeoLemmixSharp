using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class ClonerSkill : LemmingSkill
{
    public static ClonerSkill Instance { get; } = new();

    private ClonerSkill()
    {
    }

    public override int Id => GameConstants.ClonerSkillId;
    public override string LemmingSkillName => "cloner";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return BuilderAction.Instance;
        yield return MinerAction.Instance;
        yield return JumperAction.Instance;
        yield return StackerAction.Instance;
        yield return LasererAction.Instance;
        yield return SwimmerAction.Instance;
        yield return GliderAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return DiggerAction.Instance;
        yield return AscenderAction.Instance;
        yield return FallerAction.Instance;
        yield return FloaterAction.Instance;
        yield return DisarmerAction.Instance;
        yield return ShimmierAction.Instance;
        yield return ShruggerAction.Instance;
        yield return ReacherAction.Instance;
    }

    /*

    procedure TLemmingGame.GenerateClonedLem(L: TLemming);
var
  NewL: TLemming;
begin
  Assert(not L.LemIsZombie, 'cloner assigned to zombie');

  NewL := TLemming.Create;
  NewL.Assign(L);
  NewL.LemIndex := LemmingList.Count;
  NewL.LemIdentifier := 'C' + IntToStr(CurrentIteration);
  LemmingList.Add(NewL);
  TurnAround(NewL);
  Inc(LemmingsOut);

  // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
  if NewL.LemAction = baMining then
  begin
    if NewL.LemPhysicsFrame = 2 then
      ApplyMinerMask(NewL, 1, 0, 0)
    else if (NewL.LemPhysicsFrame >= 3) and (NewL.LemPhysicsFrame < 15) then
      ApplyMinerMask(NewL, 1, -2*NewL.LemDx, -1);
  end
  // Required for turned builders not to walk into air
  // For platformers, see http://www.lemmingsforums.net/index.php?topic=2530.0
  else if (NewL.LemAction in [baBuilding, baPlatforming]) and (NewL.LemPhysicsFrame >= 9) then
    LayBrick(NewL);
end;


    */
}