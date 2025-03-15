using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public static class TerrainMasks
{
    private static TerrainEraseMask _basherMask = null!;
    private static TerrainEraseMask _bomberMask = null!;
    private static TerrainEraseMask _fencerMask = null!;
    private static TerrainEraseMask _laserMask = null!;
    private static TerrainEraseMask _minerMask = null!;
    private static TerrainAddMask[] _stonerMasks = [];

    public static void InitialiseTerrainMasks(
        TerrainEraseMask basherMask,
        TerrainEraseMask bomberMask,
        TerrainEraseMask fencerMask,
        TerrainEraseMask laserMask,
        TerrainEraseMask minerMask)
    {
        if (_basherMask is not null)
            throw new InvalidOperationException("Masks have already been initialised!");

        _basherMask = basherMask;
        _bomberMask = bomberMask;
        _fencerMask = fencerMask;
        _laserMask = laserMask;
        _minerMask = minerMask;
    }

    public static void ApplyBasherMask(
        Lemming lemming,
        int frame)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        _basherMask.ApplyEraseMask(orientation, facingDirection, position, frame);
    }

    public static void ApplyBomberMask(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = orientation.MoveRight(lemming.LevelPosition, facingDirection.DeltaX);

        _bomberMask.ApplyEraseMask(orientation, facingDirection, position, 0);
    }

    public static void ApplyFencerMask(
        Lemming lemming,
        int frame)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        _fencerMask.ApplyEraseMask(orientation, facingDirection, position, frame);
    }

    public static void ApplyLasererMask(
        Lemming lemming,
        LevelPosition target)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        //  var key = GetKey(orientation, facingDirection, frame);
        //  _laserMasks[key].ApplyEraseMask(orientation, facingDirection, position);
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
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;
        var position = lemming.LevelPosition;
        position = orientation.Move(position, offsetX + dx, offsetY - frame);

        _minerMask.ApplyEraseMask(orientation, facingDirection, position, frame);
    }

    public static void ApplyStonerMask(
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        if (facingDirection == FacingDirection.Right)
        {
            position = orientation.MoveRight(position, 1);
        }

        // var key = GetKey(orientation, facingDirection, 0);
        // _stonerMasks[key].ApplyAddMask(position);
    }
}