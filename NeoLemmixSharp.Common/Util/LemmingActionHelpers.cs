using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class LemmingActionHelpers
{
    private static readonly Dictionary<string, int> LemmingActionNameToIdLookup = GenerateLemmingActionNameToIdLookup();

    private static Dictionary<string, int> GenerateLemmingActionNameToIdLookup()
    {
        var result = new Dictionary<string, int>(LemmingActionConstants.NumberOfLemmingActions, StringComparer.OrdinalIgnoreCase)
        {
            [LemmingActionConstants.WalkerActionSpriteFileName] = LemmingActionConstants.WalkerActionId,
            [LemmingActionConstants.ClimberActionSpriteFileName] = LemmingActionConstants.ClimberActionId,
            [LemmingActionConstants.FloaterActionSpriteFileName] = LemmingActionConstants.FloaterActionId,
            [LemmingActionConstants.BlockerActionSpriteFileName] = LemmingActionConstants.BlockerActionId,
            [LemmingActionConstants.BuilderActionSpriteFileName] = LemmingActionConstants.BuilderActionId,
            [LemmingActionConstants.BasherActionSpriteFileName] = LemmingActionConstants.BasherActionId,
            [LemmingActionConstants.MinerActionSpriteFileName] = LemmingActionConstants.MinerActionId,
            [LemmingActionConstants.DiggerActionSpriteFileName] = LemmingActionConstants.DiggerActionId,
            [LemmingActionConstants.PlatformerActionSpriteFileName] = LemmingActionConstants.PlatformerActionId,
            [LemmingActionConstants.StackerActionSpriteFileName] = LemmingActionConstants.StackerActionId,
            [LemmingActionConstants.FencerActionSpriteFileName] = LemmingActionConstants.FencerActionId,
            [LemmingActionConstants.GliderActionSpriteFileName] = LemmingActionConstants.GliderActionId,
            [LemmingActionConstants.JumperActionSpriteFileName] = LemmingActionConstants.JumperActionId,
            [LemmingActionConstants.SwimmerActionSpriteFileName] = LemmingActionConstants.SwimmerActionId,
            [LemmingActionConstants.ShimmierActionSpriteFileName] = LemmingActionConstants.ShimmierActionId,
            [LemmingActionConstants.LasererActionSpriteFileName] = LemmingActionConstants.LasererActionId,
            [LemmingActionConstants.SliderActionSpriteFileName] = LemmingActionConstants.SliderActionId,
            [LemmingActionConstants.FallerActionSpriteFileName] = LemmingActionConstants.FallerActionId,
            [LemmingActionConstants.AscenderActionSpriteFileName] = LemmingActionConstants.AscenderActionId,
            [LemmingActionConstants.ShruggerActionSpriteFileName] = LemmingActionConstants.ShruggerActionId,
            [LemmingActionConstants.DrownerActionSpriteFileName] = LemmingActionConstants.DrownerActionId,
            [LemmingActionConstants.HoisterActionSpriteFileName] = LemmingActionConstants.HoisterActionId,
            [LemmingActionConstants.DehoisterActionSpriteFileName] = LemmingActionConstants.DehoisterActionId,
            [LemmingActionConstants.ReacherActionSpriteFileName] = LemmingActionConstants.ReacherActionId,
            [LemmingActionConstants.DisarmerActionSpriteFileName] = LemmingActionConstants.DisarmerActionId,
            [LemmingActionConstants.ExiterActionSpriteFileName] = LemmingActionConstants.ExiterActionId,
            [LemmingActionConstants.ExploderActionSpriteFileName] = LemmingActionConstants.ExploderActionId,
            [LemmingActionConstants.OhNoerActionSpriteFileName] = LemmingActionConstants.OhNoerActionId,
            [LemmingActionConstants.SplatterActionSpriteFileName] = LemmingActionConstants.SplatterActionId,
            [LemmingActionConstants.StonerActionSpriteFileName] = LemmingActionConstants.StonerActionId,
            [LemmingActionConstants.VaporiserActionSpriteFileName] = LemmingActionConstants.VaporiserActionId,
            [LemmingActionConstants.RotateClockwiseActionSpriteFileName] = LemmingActionConstants.RotateClockwiseActionId,
            [LemmingActionConstants.RotateCounterclockwiseActionSpriteFileName] = LemmingActionConstants.RotateCounterclockwiseActionId,
            [LemmingActionConstants.RotateHalfActionSpriteFileName] = LemmingActionConstants
.RotateHalfActionId
        };

        if (result.Count != LemmingActionConstants.NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;
    }

    public static int GetLemmingActionIdFromName(string lemmingActionName)
    {
        return LemmingActionNameToIdLookup[lemmingActionName];
    }

    private static readonly LemmingActionLookupData[] LemmingActionIdToStringLookup = GenerateLemmingActionIdToStringLookup();

    private static LemmingActionLookupData[] GenerateLemmingActionIdToStringLookup()
    {
        var result = new LemmingActionLookupData[LemmingActionConstants.NumberOfLemmingActions];
        var count = 0;

        SetData(LemmingActionConstants.WalkerActionId, LemmingActionConstants.WalkerActionName, LemmingActionConstants.WalkerActionSpriteFileName, LemmingActionConstants.WalkerAnimationFrames);
        SetData(LemmingActionConstants.ClimberActionId, LemmingActionConstants.ClimberActionName, LemmingActionConstants.ClimberActionSpriteFileName, LemmingActionConstants.ClimberAnimationFrames);
        SetData(LemmingActionConstants.FloaterActionId, LemmingActionConstants.FloaterActionName, LemmingActionConstants.FloaterActionSpriteFileName, LemmingActionConstants.FloaterAnimationFrames);
        SetData(LemmingActionConstants.BlockerActionId, LemmingActionConstants.BlockerActionName, LemmingActionConstants.BlockerActionSpriteFileName, LemmingActionConstants.BlockerAnimationFrames);
        SetData(LemmingActionConstants.BuilderActionId, LemmingActionConstants.BuilderActionName, LemmingActionConstants.BuilderActionSpriteFileName, LemmingActionConstants.BuilderAnimationFrames);
        SetData(LemmingActionConstants.BasherActionId, LemmingActionConstants.BasherActionName, LemmingActionConstants.BasherActionSpriteFileName, LemmingActionConstants.BasherAnimationFrames);
        SetData(LemmingActionConstants.MinerActionId, LemmingActionConstants.MinerActionName, LemmingActionConstants.MinerActionSpriteFileName, LemmingActionConstants.MinerAnimationFrames);
        SetData(LemmingActionConstants.DiggerActionId, LemmingActionConstants.DiggerActionName, LemmingActionConstants.DiggerActionSpriteFileName, LemmingActionConstants.DiggerAnimationFrames);
        SetData(LemmingActionConstants.PlatformerActionId, LemmingActionConstants.PlatformerActionName, LemmingActionConstants.PlatformerActionSpriteFileName, LemmingActionConstants.PlatformerAnimationFrames);
        SetData(LemmingActionConstants.StackerActionId, LemmingActionConstants.StackerActionName, LemmingActionConstants.StackerActionSpriteFileName, LemmingActionConstants.StackerAnimationFrames);
        SetData(LemmingActionConstants.FencerActionId, LemmingActionConstants.FencerActionName, LemmingActionConstants.FencerActionSpriteFileName, LemmingActionConstants.FencerAnimationFrames);
        SetData(LemmingActionConstants.GliderActionId, LemmingActionConstants.GliderActionName, LemmingActionConstants.GliderActionSpriteFileName, LemmingActionConstants.GliderAnimationFrames);
        SetData(LemmingActionConstants.JumperActionId, LemmingActionConstants.JumperActionName, LemmingActionConstants.JumperActionSpriteFileName, LemmingActionConstants.JumperAnimationFrames);
        SetData(LemmingActionConstants.SwimmerActionId, LemmingActionConstants.SwimmerActionName, LemmingActionConstants.SwimmerActionSpriteFileName, LemmingActionConstants.SwimmerAnimationFrames);
        SetData(LemmingActionConstants.ShimmierActionId, LemmingActionConstants.ShimmierActionName, LemmingActionConstants.ShimmierActionSpriteFileName, LemmingActionConstants.ShimmierAnimationFrames);
        SetData(LemmingActionConstants.LasererActionId, LemmingActionConstants.LasererActionName, LemmingActionConstants.LasererActionSpriteFileName, LemmingActionConstants.LasererAnimationFrames);
        SetData(LemmingActionConstants.SliderActionId, LemmingActionConstants.SliderActionName, LemmingActionConstants.SliderActionSpriteFileName, LemmingActionConstants.SliderAnimationFrames);
        SetData(LemmingActionConstants.FallerActionId, LemmingActionConstants.FallerActionName, LemmingActionConstants.FallerActionSpriteFileName, LemmingActionConstants.FallerAnimationFrames);
        SetData(LemmingActionConstants.AscenderActionId, LemmingActionConstants.AscenderActionName, LemmingActionConstants.AscenderActionSpriteFileName, LemmingActionConstants.AscenderAnimationFrames);
        SetData(LemmingActionConstants.ShruggerActionId, LemmingActionConstants.ShruggerActionName, LemmingActionConstants.ShruggerActionSpriteFileName, LemmingActionConstants.ShruggerAnimationFrames);
        SetData(LemmingActionConstants.DrownerActionId, LemmingActionConstants.DrownerActionName, LemmingActionConstants.DrownerActionSpriteFileName, LemmingActionConstants.DrownerAnimationFrames);
        SetData(LemmingActionConstants.HoisterActionId, LemmingActionConstants.HoisterActionName, LemmingActionConstants.HoisterActionSpriteFileName, LemmingActionConstants.HoisterAnimationFrames);
        SetData(LemmingActionConstants.DehoisterActionId, LemmingActionConstants.DehoisterActionName, LemmingActionConstants.DehoisterActionSpriteFileName, LemmingActionConstants.DehoisterAnimationFrames);
        SetData(LemmingActionConstants.ReacherActionId, LemmingActionConstants.ReacherActionName, LemmingActionConstants.ReacherActionSpriteFileName, LemmingActionConstants.ReacherAnimationFrames);
        SetData(LemmingActionConstants.DisarmerActionId, LemmingActionConstants.DisarmerActionName, LemmingActionConstants.DisarmerActionSpriteFileName, LemmingActionConstants.DisarmerAnimationFrames);
        SetData(LemmingActionConstants.ExiterActionId, LemmingActionConstants.ExiterActionName, LemmingActionConstants.ExiterActionSpriteFileName, LemmingActionConstants.ExiterAnimationFrames);
        SetData(LemmingActionConstants.ExploderActionId, LemmingActionConstants.ExploderActionName, LemmingActionConstants.ExploderActionSpriteFileName, LemmingActionConstants.ExploderAnimationFrames);
        SetData(LemmingActionConstants.OhNoerActionId, LemmingActionConstants.OhNoerActionName, LemmingActionConstants.OhNoerActionSpriteFileName, LemmingActionConstants.OhNoerAnimationFrames);
        SetData(LemmingActionConstants.SplatterActionId, LemmingActionConstants.SplatterActionName, LemmingActionConstants.SplatterActionSpriteFileName, LemmingActionConstants.SplatterAnimationFrames);
        SetData(LemmingActionConstants.StonerActionId, LemmingActionConstants.StonerActionName, LemmingActionConstants.StonerActionSpriteFileName, LemmingActionConstants.StonerAnimationFrames);
        SetData(LemmingActionConstants.VaporiserActionId, LemmingActionConstants.VaporiserActionName, LemmingActionConstants.VaporiserActionSpriteFileName, LemmingActionConstants.VaporiserAnimationFrames);
        SetData(LemmingActionConstants.RotateClockwiseActionId, LemmingActionConstants.RotateClockwiseActionName, LemmingActionConstants.RotateClockwiseActionSpriteFileName, LemmingActionConstants.RotateClockwiseAnimationFrames);
        SetData(LemmingActionConstants.RotateCounterclockwiseActionId, LemmingActionConstants.RotateCounterclockwiseActionName, LemmingActionConstants.RotateCounterclockwiseActionSpriteFileName, LemmingActionConstants.RotateCounterclockwiseAnimationFrames);
        SetData(LemmingActionConstants.RotateHalfActionId, LemmingActionConstants.RotateHalfActionName, LemmingActionConstants.RotateHalfActionSpriteFileName, LemmingActionConstants.RotateHalfAnimationFrames);

        if (count != LemmingActionConstants.NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetData(int index, string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
        {
            result[index] = new LemmingActionLookupData(lemmingActionName, lemmingActionFileName, numberOfAnimationFrames);
            count++;
        }
    }

    public static LemmingActionLookupData GetLemmingActionDataFromId(int lemmingActionId) => LemmingActionIdToStringLookup[lemmingActionId];

    public readonly struct LemmingActionLookupData(string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
    {
        public readonly string LemmingActionName = lemmingActionName;
        public readonly string LemmingActionFileName = lemmingActionFileName;
        public readonly int NumberOfAnimationFrames = numberOfAnimationFrames;
    }
}
