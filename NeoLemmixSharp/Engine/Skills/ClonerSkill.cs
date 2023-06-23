using NeoLemmixSharp.Engine.Actions;

namespace NeoLemmixSharp.Engine.Skills;

public sealed class ClonerSkill : LemmingSkill
{
    public ClonerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 5;
    public override string LemmingSkillName => "cloner";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == JumperAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance ||
               lemming.CurrentAction == SwimmerAction.Instance ||
               lemming.CurrentAction == GliderAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == AscenderAction.Instance ||
               lemming.CurrentAction == FallerAction.Instance ||
               lemming.CurrentAction == FloaterAction.Instance ||
               lemming.CurrentAction == DisarmerAction.Instance ||
               lemming.CurrentAction == ShimmierAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == ReacherAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        // Inc(LemmingsCloned);




        return true;
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