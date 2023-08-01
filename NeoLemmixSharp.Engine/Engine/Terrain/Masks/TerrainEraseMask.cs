using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Terrain.Masks;

public sealed class TerrainEraseMask
{
#pragma warning disable CS8618
    private static TerrainManager _terrainManager;
#pragma warning restore CS8618

    private readonly IDestructionAction _destructionAction;
    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;

    public TerrainEraseMask(
        IDestructionAction destructionAction,
        LevelPosition anchorPoint,
        LevelPosition[] mask)
    {
        _destructionAction = destructionAction;
        _anchorPoint = anchorPoint;
        _mask = mask;
    }

    public void ApplyEraseMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position)
    {
        var offset = position - _anchorPoint;

        for (var i = 0; i < _mask.Length; i++)
        {
            var pixel = _mask[i];

            pixel += offset;

            _terrainManager.ErasePixel(orientation, _destructionAction, facingDirection, pixel);
        }
    }

    public static void SetTerrain(TerrainManager manager)
    {
        _terrainManager = manager;
    }
}