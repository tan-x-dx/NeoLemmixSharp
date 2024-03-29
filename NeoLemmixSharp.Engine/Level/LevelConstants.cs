﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.PositionTracking;

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
    public const int ClassicTeamId = 0;
    public const int NumberOfClassicSkills = 8;

    public const int InfiniteSkillCount = 100;

    public const int InitialLemmingHatchReleaseCountDown = 20;

    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    public const int MaxFallDistance = 62;

    public const int DefaultCountDownTimer = 5;
    public const int DefaultCountDownActionTicks = DefaultCountDownTimer * EngineConstants.StandardTicksPerSecond;
    public const int DefaultFastForwardLemmingCountDownActionTicks = EngineConstants.FastForwardSpeedMultiplier * DefaultCountDownActionTicks;

    public const int ParticleFrameCount = 51;
    public const int NumberOfParticles = 80;

    public const int MinAllowedSpawnInterval = 4;
    public const int MaxAllowedSpawnInterval = 102;

    public const int BlockerQuantityThreshold = 20;

    public const int MaxNumberOfLemmings = 500;
    public const int MaxTimeLimitInSeconds = 99 * 60 + 59; // 99 minutes, 59 seconds
    public const int MaxLevelWidth = 2400;
    public const int MaxLevelHeight = 2400;

    /// <summary>
    /// A lemming falls 3 pixels each frame
    /// </summary>
    public const int DefaultFallStep = 3;
    /// <summary>
    /// A lemming falls 2 pixels each frame if there's an updraft at its location
    /// </summary>
    public const int UpdraftFallStep = 2;
    /// <summary>
    /// A lemming falls 4 pixels each frame if there's a downdraft at its location
    /// </summary>
    public const int DownDraftFallStep = 4;

    #endregion

    #region Default Colours

    private static readonly Color[] ExplosionParticleColors =
    [
        new Color(0x40, 0x40, 0xE0, 0xFF),
        new Color(0x00, 0xB0, 0x00, 0xFF),
        new Color(0xF0, 0xD0, 0xD0, 0xFF),
        new Color(0xF0, 0x20, 0x20, 0xFF),
        new Color(0x40, 0x40, 0xE0, 0xC0),
        new Color(0x00, 0xB0, 0x00, 0xC0),
        new Color(0xF0, 0xD0, 0xD0, 0xC0),
        new Color(0xF0, 0x20, 0x20, 0xC0)
    ];

    public static ReadOnlySpan<Color> GetExplosionParticleColors() => new(ExplosionParticleColors);
    public const int NumberOfExplosionParticleColors = 8;
    public const int NumberOfExplosionParticleColorsMask = NumberOfExplosionParticleColors - 1;

    public static Color CursorColor1 => new(0xB0, 0xB0, 0xB0);
    public static Color CursorColor2 => PanelRed;
    public static Color CursorColor3 => new(0x60, 0x60, 0x60);

    public static Color PanelGreen => new(0x00, 0xB0, 0x00);
    public static Color PanelYellow => new(0xB0, 0xB0, 0x00);
    public static Color PanelRed => new(0xB0, 0x00, 0x00);
    public static Color PanelMagenta => new(0xB0, 0x00, 0xB0);

    #endregion

    #region Position Tracking Data

    public const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize16;
    public const ChunkSizeType GadgetPositionChunkSize = ChunkSizeType.ChunkSize64;

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

    public const int FireGadgetBehaviourId = 2;
    public const int FunctionalGadgetBehaviourId = 10;
    public const int GenericGadgetBehaviourId = 0;
    public const int HatchGadgetBehaviourId = 12;
    public const int LogicGateGadgetBehaviourId = 11;
    public const int MetalGrateGadgetBehaviourId = 9;
    public const int NoSplatGadgetBehaviourId = 6;
    public const int SawBladeGadgetBehaviourId = 8;
    public const int SplatGadgetBehaviourId = 5;
    public const int SwitchGadgetBehaviourId = 7;
    public const int TinkerableGadgetBehaviourId = 3;
    public const int UpdraftGadgetBehaviourId = 4;
    public const int WaterGadgetBehaviourId = 1;

    #endregion
}