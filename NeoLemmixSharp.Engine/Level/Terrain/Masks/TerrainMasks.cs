using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public static partial class TerrainMasks
{
#pragma warning disable CS8618
	private static TerrainEraseMask[] _basherMasks;
	private static TerrainEraseMask[] _bomberMasks;
	private static TerrainEraseMask[] _fencerMasks;
	private static TerrainEraseMask[] _laserMasks;
	private static TerrainEraseMask[] _minerMasks;
	private static TerrainAddMask[] _stonerMasks;
#pragma warning restore CS8618

	public static void ApplyBasherMask(
		Lemming lemming,
		int frame)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var position = lemming.LevelPosition;

		var key = GetKey(orientation, facingDirection, frame);
		_basherMasks[key].ApplyEraseMask(orientation, facingDirection, position);
	}

	public static void ApplyBomberMask(Lemming lemming)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var position = lemming.LevelPosition;

		if (facingDirection == FacingDirection.RightInstance)
		{
			position = orientation.MoveRight(position, 1);
		}

		var key = GetKey(orientation, facingDirection, 0);
		_bomberMasks[key].ApplyEraseMask(orientation, facingDirection, position);
	}

	public static void ApplyFencerMask(
		Lemming lemming,
		int frame)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var position = lemming.LevelPosition;

		var key = GetKey(orientation, facingDirection, frame);
		_fencerMasks[key].ApplyEraseMask(orientation, facingDirection, position);
	}

	public static void ApplyLasererMask(
		Lemming lemming,
		int frame)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var position = lemming.LevelPosition;

		var key = GetKey(orientation, facingDirection, frame);
		_laserMasks[key].ApplyEraseMask(orientation, facingDirection, position);
	}

	/// <summary>
	/// The miner mask is usually centered at the feet of the lemming. The offset parameters changes the position of the miner mask relative to this.
	///
	/// This method deals with lemming orientations by itself!
	/// </summary>
	public static void ApplyMinerMask(
		Lemming lemming,
		int offsetX,
		int offsetY,
		int frame)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var dx = facingDirection.DeltaX;
		var position = lemming.LevelPosition;
		position = orientation.Move(position, offsetX + dx, offsetY - frame);

		var key = GetKey(orientation, facingDirection, frame);
		_minerMasks[key].ApplyEraseMask(orientation, facingDirection, position);
	}

	public static void ApplyStonerMask(
		Lemming lemming)
	{
		var orientation = lemming.Orientation;
		var facingDirection = lemming.FacingDirection;
		var position = lemming.LevelPosition;

		if (facingDirection == FacingDirection.RightInstance)
		{
			position = orientation.MoveRight(position, 1);
		}

		var key = GetKey(orientation, facingDirection, 0);
		_stonerMasks[key].ApplyAddMask(position);
	}
}