using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class LemmingActionHelpers
{
    private static readonly Dictionary<string, int> LemmingActionNameToIdLookup = GenerateLemmingActionNameToIdLookup();

    private static Dictionary<string, int> GenerateLemmingActionNameToIdLookup()
    {
        var result = new Dictionary<string, int>(EngineConstants.NumberOfLemmingActions, StringComparer.OrdinalIgnoreCase)
        {
            [EngineConstants.WalkerActionSpriteFileName] = EngineConstants.WalkerActionId,
            [EngineConstants.ClimberActionSpriteFileName] = EngineConstants.ClimberActionId,
            [EngineConstants.FloaterActionSpriteFileName] = EngineConstants.FloaterActionId,
            [EngineConstants.BlockerActionSpriteFileName] = EngineConstants.BlockerActionId,
            [EngineConstants.BuilderActionSpriteFileName] = EngineConstants.BuilderActionId,
            [EngineConstants.BasherActionSpriteFileName] = EngineConstants.BasherActionId,
            [EngineConstants.MinerActionSpriteFileName] = EngineConstants.MinerActionId,
            [EngineConstants.DiggerActionSpriteFileName] = EngineConstants.DiggerActionId,
            [EngineConstants.PlatformerActionSpriteFileName] = EngineConstants.PlatformerActionId,
            [EngineConstants.StackerActionSpriteFileName] = EngineConstants.StackerActionId,
            [EngineConstants.FencerActionSpriteFileName] = EngineConstants.FencerActionId,
            [EngineConstants.GliderActionSpriteFileName] = EngineConstants.GliderActionId,
            [EngineConstants.JumperActionSpriteFileName] = EngineConstants.JumperActionId,
            [EngineConstants.SwimmerActionSpriteFileName] = EngineConstants.SwimmerActionId,
            [EngineConstants.ShimmierActionSpriteFileName] = EngineConstants.ShimmierActionId,
            [EngineConstants.LasererActionSpriteFileName] = EngineConstants.LasererActionId,
            [EngineConstants.SliderActionSpriteFileName] = EngineConstants.SliderActionId,
            [EngineConstants.FallerActionSpriteFileName] = EngineConstants.FallerActionId,
            [EngineConstants.AscenderActionSpriteFileName] = EngineConstants.AscenderActionId,
            [EngineConstants.ShruggerActionSpriteFileName] = EngineConstants.ShruggerActionId,
            [EngineConstants.DrownerActionSpriteFileName] = EngineConstants.DrownerActionId,
            [EngineConstants.HoisterActionSpriteFileName] = EngineConstants.HoisterActionId,
            [EngineConstants.DehoisterActionSpriteFileName] = EngineConstants.DehoisterActionId,
            [EngineConstants.ReacherActionSpriteFileName] = EngineConstants.ReacherActionId,
            [EngineConstants.DisarmerActionSpriteFileName] = EngineConstants.DisarmerActionId,
            [EngineConstants.ExiterActionSpriteFileName] = EngineConstants.ExiterActionId,
            [EngineConstants.ExploderActionSpriteFileName] = EngineConstants.ExploderActionId,
            [EngineConstants.OhNoerActionSpriteFileName] = EngineConstants.OhNoerActionId,
            [EngineConstants.SplatterActionSpriteFileName] = EngineConstants.SplatterActionId,
            [EngineConstants.StonerActionSpriteFileName] = EngineConstants.StonerActionId,
            [EngineConstants.VaporiserActionSpriteFileName] = EngineConstants.VaporiserActionId,
            [EngineConstants.RotateClockwiseActionSpriteFileName] = EngineConstants.RotateClockwiseActionId,
            [EngineConstants.RotateCounterclockwiseActionSpriteFileName] = EngineConstants.RotateCounterclockwiseActionId,
            [EngineConstants.RotateHalfActionSpriteFileName] = EngineConstants.RotateHalfActionId
        };

        if (result.Count != EngineConstants.NumberOfLemmingActions)
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
        var result = new LemmingActionLookupData[EngineConstants.NumberOfLemmingActions];
        var count = 0;

        SetData(EngineConstants.WalkerActionId, EngineConstants.WalkerActionName, EngineConstants.WalkerActionSpriteFileName, EngineConstants.WalkerAnimationFrames);
        SetData(EngineConstants.ClimberActionId, EngineConstants.ClimberActionName, EngineConstants.ClimberActionSpriteFileName, EngineConstants.ClimberAnimationFrames);
        SetData(EngineConstants.FloaterActionId, EngineConstants.FloaterActionName, EngineConstants.FloaterActionSpriteFileName, EngineConstants.FloaterAnimationFrames);
        SetData(EngineConstants.BlockerActionId, EngineConstants.BlockerActionName, EngineConstants.BlockerActionSpriteFileName, EngineConstants.BlockerAnimationFrames);
        SetData(EngineConstants.BuilderActionId, EngineConstants.BuilderActionName, EngineConstants.BuilderActionSpriteFileName, EngineConstants.BuilderAnimationFrames);
        SetData(EngineConstants.BasherActionId, EngineConstants.BasherActionName, EngineConstants.BasherActionSpriteFileName, EngineConstants.BasherAnimationFrames);
        SetData(EngineConstants.MinerActionId, EngineConstants.MinerActionName, EngineConstants.MinerActionSpriteFileName, EngineConstants.MinerAnimationFrames);
        SetData(EngineConstants.DiggerActionId, EngineConstants.DiggerActionName, EngineConstants.DiggerActionSpriteFileName, EngineConstants.DiggerAnimationFrames);
        SetData(EngineConstants.PlatformerActionId, EngineConstants.PlatformerActionName, EngineConstants.PlatformerActionSpriteFileName, EngineConstants.PlatformerAnimationFrames);
        SetData(EngineConstants.StackerActionId, EngineConstants.StackerActionName, EngineConstants.StackerActionSpriteFileName, EngineConstants.StackerAnimationFrames);
        SetData(EngineConstants.FencerActionId, EngineConstants.FencerActionName, EngineConstants.FencerActionSpriteFileName, EngineConstants.FencerAnimationFrames);
        SetData(EngineConstants.GliderActionId, EngineConstants.GliderActionName, EngineConstants.GliderActionSpriteFileName, EngineConstants.GliderAnimationFrames);
        SetData(EngineConstants.JumperActionId, EngineConstants.JumperActionName, EngineConstants.JumperActionSpriteFileName, EngineConstants.JumperAnimationFrames);
        SetData(EngineConstants.SwimmerActionId, EngineConstants.SwimmerActionName, EngineConstants.SwimmerActionSpriteFileName, EngineConstants.SwimmerAnimationFrames);
        SetData(EngineConstants.ShimmierActionId, EngineConstants.ShimmierActionName, EngineConstants.ShimmierActionSpriteFileName, EngineConstants.ShimmierAnimationFrames);
        SetData(EngineConstants.LasererActionId, EngineConstants.LasererActionName, EngineConstants.LasererActionSpriteFileName, EngineConstants.LasererAnimationFrames);
        SetData(EngineConstants.SliderActionId, EngineConstants.SliderActionName, EngineConstants.SliderActionSpriteFileName, EngineConstants.SliderAnimationFrames);
        SetData(EngineConstants.FallerActionId, EngineConstants.FallerActionName, EngineConstants.FallerActionSpriteFileName, EngineConstants.FallerAnimationFrames);
        SetData(EngineConstants.AscenderActionId, EngineConstants.AscenderActionName, EngineConstants.AscenderActionSpriteFileName, EngineConstants.AscenderAnimationFrames);
        SetData(EngineConstants.ShruggerActionId, EngineConstants.ShruggerActionName, EngineConstants.ShruggerActionSpriteFileName, EngineConstants.ShruggerAnimationFrames);
        SetData(EngineConstants.DrownerActionId, EngineConstants.DrownerActionName, EngineConstants.DrownerActionSpriteFileName, EngineConstants.DrownerAnimationFrames);
        SetData(EngineConstants.HoisterActionId, EngineConstants.HoisterActionName, EngineConstants.HoisterActionSpriteFileName, EngineConstants.HoisterAnimationFrames);
        SetData(EngineConstants.DehoisterActionId, EngineConstants.DehoisterActionName, EngineConstants.DehoisterActionSpriteFileName, EngineConstants.DehoisterAnimationFrames);
        SetData(EngineConstants.ReacherActionId, EngineConstants.ReacherActionName, EngineConstants.ReacherActionSpriteFileName, EngineConstants.ReacherAnimationFrames);
        SetData(EngineConstants.DisarmerActionId, EngineConstants.DisarmerActionName, EngineConstants.DisarmerActionSpriteFileName, EngineConstants.DisarmerAnimationFrames);
        SetData(EngineConstants.ExiterActionId, EngineConstants.ExiterActionName, EngineConstants.ExiterActionSpriteFileName, EngineConstants.ExiterAnimationFrames);
        SetData(EngineConstants.ExploderActionId, EngineConstants.ExploderActionName, EngineConstants.ExploderActionSpriteFileName, EngineConstants.ExploderAnimationFrames);
        SetData(EngineConstants.OhNoerActionId, EngineConstants.OhNoerActionName, EngineConstants.OhNoerActionSpriteFileName, EngineConstants.OhNoerAnimationFrames);
        SetData(EngineConstants.SplatterActionId, EngineConstants.SplatterActionName, EngineConstants.SplatterActionSpriteFileName, EngineConstants.SplatterAnimationFrames);
        SetData(EngineConstants.StonerActionId, EngineConstants.StonerActionName, EngineConstants.StonerActionSpriteFileName, EngineConstants.StonerAnimationFrames);
        SetData(EngineConstants.VaporiserActionId, EngineConstants.VaporiserActionName, EngineConstants.VaporiserActionSpriteFileName, EngineConstants.VaporiserAnimationFrames);
        SetData(EngineConstants.RotateClockwiseActionId, EngineConstants.RotateClockwiseActionName, EngineConstants.RotateClockwiseActionSpriteFileName, EngineConstants.RotateClockwiseAnimationFrames);
        SetData(EngineConstants.RotateCounterclockwiseActionId, EngineConstants.RotateCounterclockwiseActionName, EngineConstants.RotateCounterclockwiseActionSpriteFileName, EngineConstants.RotateCounterclockwiseAnimationFrames);
        SetData(EngineConstants.RotateHalfActionId, EngineConstants.RotateHalfActionName, EngineConstants.RotateHalfActionSpriteFileName, EngineConstants.RotateHalfAnimationFrames);

        if (count != EngineConstants.NumberOfLemmingActions)
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
