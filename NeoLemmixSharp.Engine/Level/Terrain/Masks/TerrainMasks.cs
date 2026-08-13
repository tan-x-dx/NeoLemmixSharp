using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public static class TerrainMasks
{
    public static TerrainEraseMask BasherMask { get; private set; } = null!;
    public static TerrainEraseMask BomberMask { get; private set; } = null!;
    public static TerrainEraseMask FencerMask { get; private set; } = null!;
    public static TerrainEraseMask LaserMask { get; private set; } = null!;
    public static TerrainEraseMask MinerMask { get; private set; } = null!;
    public static TerrainAddMask[] StonerMask { get; private set; } = null!;

    private static PixelType[] BasherSimulationScratchSpace = null!;

    public static void InitialiseTerrainMasks(
        TerrainEraseMask basherMask,
        TerrainEraseMask bomberMask,
        TerrainEraseMask fencerMask,
        TerrainEraseMask laserMask,
        TerrainEraseMask minerMask)
    {
        if (BasherMask is not null)
            Helpers.ThrowMultipleInitialisationError(nameof(TerrainMasks));

        BasherMask = basherMask;
        BomberMask = bomberMask;
        FencerMask = fencerMask;
        LaserMask = laserMask;
        MinerMask = minerMask;

        BasherSimulationScratchSpace = new PixelType[basherMask.Dimensions.Size.Area()];
    }

    public static void ApplyBasherMask(
        Lemming lemming,
        int frame)
    {
        var dht = lemming.DihedralTransformation;
        var position = lemming.AnchorPosition;

        BasherMask.ApplyEraseMask(dht, position, frame);
    }

    public static void GetBasherSimulationScratchSpace(
        Lemming lemming,
        out ArrayWrapper2D<PixelType> scratchSpaceData)
    {
        var dht = lemming.DihedralTransformation;
        var terrainManager = LevelScreen.TerrainManager;

        var sourceRegion = BasherMask.Dimensions.Translate(lemming.AnchorPosition);

        var source = new ArrayWrapper2D<PixelType>(terrainManager.RawPixels, terrainManager.LevelDimensions, sourceRegion);
        scratchSpaceData = new ArrayWrapper2D<PixelType>(BasherSimulationScratchSpace, BasherMask.Dimensions.Size);

        ArrayWrapper2D<PixelType>.CopyTo(in source, in scratchSpaceData, dht);
    }

    public static void ApplyBomberMask(Lemming lemming)
    {
        var dht = lemming.DihedralTransformation;
        var position = dht.Orientation.MoveRight(lemming.AnchorPosition, dht.FacingDirection.DeltaX);

        BomberMask.ApplyEraseMask(dht, position, 0);
    }

    public static void ApplyFencerMask(
        Lemming lemming,
        int frame)
    {
        var dht = lemming.DihedralTransformation;
        var position = lemming.AnchorPosition;

        FencerMask.ApplyEraseMask(dht, position, frame);
    }

    public static void ApplyLasererMask(
        Lemming lemming,
        Point target)
    {
        var dht = lemming.DihedralTransformation;
        var position = lemming.AnchorPosition;

        //  var key = GetKey(dht, frame);
        //  _laserMasks[key].ApplyEraseMask(dht, position);
    }

    /// <summary>
    /// The miner mask is usually centered at the feet of the lemming. The offset parameters changes the position of the miner mask relative to this.
    ///
    /// This method deals with lemming orientations by itself!
    /// </summary>
    public static void ApplyMinerMask(Lemming lemming,
        int frame,
        int offsetX,
        int offsetY)
    {
        var dht = lemming.DihedralTransformation;
        var dx = dht.FacingDirection.DeltaX;
        var position = lemming.AnchorPosition;
        position = dht.Orientation.Move(position, offsetX + dx, offsetY - frame);

        MinerMask.ApplyEraseMask(dht, position, frame);
    }

    public static void ApplyStonerMask(
        Lemming lemming)
    {
        var dht = lemming.DihedralTransformation;
        var position = lemming.AnchorPosition;

        if (dht.FacingDirection == FacingDirection.Right)
        {
            position = dht.Orientation.MoveRight(position, 1);
        }

        // var key = GetKey(dht, 0);
        // _stonerMasks[key].ApplyAddMask(position);
    }
}