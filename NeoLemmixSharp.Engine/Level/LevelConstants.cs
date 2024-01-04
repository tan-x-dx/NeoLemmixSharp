using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Level;

public static class LevelConstants
{
	#region Engine Constants

	public const int RightFacingDirectionId = 0;
	public const int RightFacingDirectionDeltaX = 1;

	public const int LeftFacingDirectionId = 1;
	public const int LeftFacingDirectionDeltaX = -1;

	public const int DownOrientationRotNum = 0;
	public const int LeftOrientationRotNum = 1;
	public const int UpOrientationRotNum = 2;
	public const int RightOrientationRotNum = 3;

	public const int NumberOfTeams = 6;
	public const int NumberOfClassicSkills = 8;

	public const int InfiniteSkillCount = 100;

	public const int InitialLemmingCountDown = 20;

	public const int CursorSizeInPixels = 16;
	public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

	#endregion

	#region Default Colours

	public static Color CursorColor1 => new(0xff, 0xff, 0xff);
	public static Color CursorColor2 => new(0xff, 0x00, 0x00);
	public static Color CursorColor3 => new(0x88, 0x88, 0x88);

	#endregion

	#region Cursor Priority Levels

	public const int NonPermanentSkillPriority = 4;
	public const int PermanentSkillPriority = 3;
	public const int NonWalkerMovementPriority = 2;
	public const int WalkerMovementPriority = 1;
	public const int NoPriority = 0;

	#endregion

	#region Lemming Action Constants

	public const int AscenderActionId = 18;
	public const int AscenderAnimationFrames = 1;

	public const int BasherActionId = 5;
	public const int BasherAnimationFrames = 32;

	public const int BlockerActionId = 3;
	public const int BlockerAnimationFrames = 16;

	public const int BuilderActionId = 4;
	public const int BuilderAnimationFrames = 16;

	public const int ClimberActionId = 1;
	public const int ClimberAnimationFrames = 8;

	public const int DehoisterActionId = 22;
	public const int DehoisterAnimationFrames = 7;

	public const int DiggerActionId = 7;
	public const int DiggerAnimationFrames = 16;

	public const int DisarmerActionId = 24;
	public const int DisarmerAnimationFrames = 16;

	public const int DrownerActionId = 20;
	public const int DrownerAnimationFrames = 16;

	public const int ExiterActionId = 25;
	public const int ExiterAnimationFrames = 8;

	public const int ExploderActionId = 26;
	public const int ExploderAnimationFrames = 1;

	public const int FallerActionId = 17;
	public const int FallerAnimationFrames = 4;

	public const int FencerActionId = 10;
	public const int FencerAnimationFrames = 16;

	public const int FloaterActionId = 2;
	public const int FloaterAnimationFrames = 17;

	public const int GliderActionId = 11;
	public const int GliderAnimationFrames = 17;

	public const int HoisterActionId = 21;
	public const int HoisterAnimationFrames = 8;

	public const int JumperActionId = 12;
	public const int JumperAnimationFrames = 13;

	public const int LasererActionId = 15;
	public const int LasererAnimationFrames = 1;

	public const int MinerActionId = 6;
	public const int MinerAnimationFrames = 24;

	public const int OhNoerActionId = 27;
	public const int OhNoerAnimationFrames = 16;

	public const int PlatformerActionId = 8;
	public const int PlatformerAnimationFrames = 16;

	public const int ReacherActionId = 23;
	public const int ReacherAnimationFrames = 8;

	public const int ShimmierActionId = 14;
	public const int ShimmierAnimationFrames = 20;

	public const int ShruggerActionId = 19;
	public const int ShruggerAnimationFrames = 8;

	public const int SliderActionId = 16;
	public const int SliderAnimationFrames = 1;

	public const int SplatterActionId = 28;
	public const int SplatterAnimationFrames = 16;

	public const int StackerActionId = 9;
	public const int StackerAnimationFrames = 8;

	public const int StonerActionId = 29;
	public const int StonerAnimationFrames = 16;

	public const int SwimmerActionId = 13;
	public const int SwimmerAnimationFrames = 8;

	public const int VaporiserActionId = 30;
	public const int VaporiserAnimationFrames = 16;

	public const int WalkerActionId = 0;
	public const int WalkerAnimationFrames = 8;

	#endregion

	#region Lemming Skill Constants

	public const int BasherSkillId = 5;
	public const int BlockerSkillId = 2;
	public const int BomberSkillId = 3;
	public const int BuilderSkillId = 4;
	public const int ClimberSkillId = 0;
	public const int ClonerSkillId = 20;
	public const int DiggerSkillId = 7;
	public const int DisarmerSkillId = 18;
	public const int FencerSkillId = 11;
	public const int FloaterSkillId = 1;
	public const int GliderSkillId = 12;
	public const int JumperSkillId = 13;
	public const int LasererSkillId = 16;
	public const int MinerSkillId = 6;
	public const int PlatformerSkillId = 9;
	public const int ShimmierSkillId = 15;
	public const int SliderSkillId = 17;
	public const int StackerSkillId = 10;
	public const int StonerSkillId = 19;
	public const int SwimmerSkillId = 14;
	public const int WalkerSkillId = 8;

	#endregion

	#region Gadget Constants

	public const int FireGadgetTypeId = 2;
	public const int FunctionalGadgetTypeId = 10;
	public const int GenericGadgetTypeId = 0;
	public const int HatchGadgetTypeId = 12;
	public const int LogicGateGadgetTypeId = 11;
	public const int MetalGrateGadgetTypeId = 9;
	public const int NoSplatGadgetTypeId = 6;
	public const int SawBladeGadgetTypeId = 8;
	public const int SplatGadgetTypeId = 5;
	public const int SwitchGadgetTypeId = 7;
	public const int TinkerableGadgetTypeId = 3;
	public const int UpdraftGadgetTypeId = 4;
	public const int WaterGadgetTypeId = 1;

	#endregion
}