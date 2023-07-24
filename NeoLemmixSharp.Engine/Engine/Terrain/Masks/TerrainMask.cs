using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Runtime.CompilerServices;
using MaskCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<NeoLemmixSharp.Engine.Engine.Terrain.Masks.TerrainMaskTextureReader>;

namespace NeoLemmixSharp.Engine.Engine.Terrain.Masks;

public sealed class TerrainMask
{
#pragma warning disable CS8618
    private static TerrainManager _terrainManager;
#pragma warning restore CS8618

    private readonly IDestructionAction _destructionAction;
    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;

    public TerrainMask(
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

public static class TerrainMasks
{
#pragma warning disable CS8618
    private static TerrainMask[] _basherMasks;
    private static TerrainMask[] _bomberMasks;
    private static TerrainMask[] _fencerMasks;
    private static TerrainMask[] _laserMasks;
    private static TerrainMask[] _minerMasks;
    private static TerrainMask[] _stonerMasks;
#pragma warning restore CS8618

    public static void InitialiseTerrainMasks(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        var maskCreator = new MaskCreator(graphicsDevice);

        _basherMasks = CreateTerrainMaskArray(
            BasherAction.Instance,
            "basher",
            new LevelPosition(8, 10),
            4);

        _bomberMasks = CreateTerrainMaskArray(
            ExploderAction.Instance,
            "bomber",
            new LevelPosition(16, 25),
            1);

        _fencerMasks = CreateTerrainMaskArray(
            FencerAction.Instance,
            "fencer",
            new LevelPosition(5, 10),
            4);

        /* _laserMasks = CreateTerrainMaskArray(
             LasererAction.Instance,
             "laser",
             new LevelPosition(3, 10),
             1);*/

        _minerMasks = CreateTerrainMaskArray(
            MinerAction.Instance,
            "miner",
            new LevelPosition(1, 12),
            2);

        /* _stonerMasks = CreateTerrainMaskArray(
             StonerAction.Instance,
             "stoner",
             new LevelPosition(16, 25),
             1);*/

        TerrainMask[] CreateTerrainMaskArray(
            IDestructionAction destructionAction,
            string actionName,
            LevelPosition anchorPoint,
            int numberOfFrames)
        {
            return CreateTerrainMasks(
                contentManager,
                maskCreator,
                destructionAction,
                actionName,
                anchorPoint,
                numberOfFrames);
        }
    }

    private static TerrainMask[] CreateTerrainMasks(
        ContentManager contentManager,
        MaskCreator maskCreator,
        IDestructionAction destructionAction,
        string actionName,
        LevelPosition anchorPoint,
        int numberOfFrames)
    {
        using var texture = contentManager.Load<Texture2D>($"mask/{actionName}");

        var spriteWidth = texture.Width;
        var spriteHeight = texture.Height / numberOfFrames;

        var itemCreator = CreateTerrainMaskFromTexture(destructionAction);

        var terrainMaskTextureReaders = maskCreator.CreateAllSpriteTypes(
            texture,
            spriteWidth,
            spriteHeight,
            numberOfFrames,
            1,
            anchorPoint,
            itemCreator);

        var result = new TerrainMask[numberOfFrames * 4 * 2];
        // Number of frames * 4 orientations * 2 facing directions.

        for (var f = 0; f < numberOfFrames; f++)
        {
            foreach (var orientation in Orientation.AllOrientations)
            {
                var k0 = GetKey(orientation, RightFacingDirection.Instance);
                var k1 = GetKey(orientation, RightFacingDirection.Instance, f);

                var terrainMaskTextureReader = terrainMaskTextureReaders[k0];
                var terrainMaskFrame = terrainMaskTextureReader.TerrainMaskForFrame(f);

                result[k1] = terrainMaskFrame;

                k0 = GetKey(orientation, LeftFacingDirection.Instance);
                k1 = GetKey(orientation, LeftFacingDirection.Instance, f);

                terrainMaskTextureReader = terrainMaskTextureReaders[k0];
                terrainMaskFrame = terrainMaskTextureReader.TerrainMaskForFrame(f);

                result[k1] = terrainMaskFrame;
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetKey(
        Orientation orientation,
        FacingDirection facingDirection,
        int frame)
    {
        var lowerBits = GetKey(orientation, facingDirection);

        return (frame << 3) | lowerBits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetKey(
        Orientation orientation,
        FacingDirection facingDirection)
    {
        return (orientation.RotNum << 1) | facingDirection.Id;
    }

    private static MaskCreator.ItemCreator CreateTerrainMaskFromTexture(IDestructionAction destructionAction)
    {
        // Currying is such fun...
        return (t, w, h, f, _, p) => new TerrainMaskTextureReader(t, destructionAction, w, h, f, p);
    }

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

    public static void ApplyBomberMask(
        Lemming lemming,
        int frame)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        if (facingDirection == RightFacingDirection.Instance)
        {
            position = orientation.MoveRight(position, 1);
        }

        var key = GetKey(orientation, facingDirection, frame);
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
    /// The miner mask is usually centered at the feet of the lemming. The adjustment parameter changes the position of the miner mask relative to this
    /// </summary>
    public static void ApplyMinerMask(
        Lemming lemming,
        LevelPosition adjustment,
        int frame)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;
        var position = lemming.LevelPosition;
        position = orientation.Move(position, dx, -frame);
        position = orientation.Move(position, adjustment);

        var key = GetKey(orientation, facingDirection, frame);
        _minerMasks[key].ApplyEraseMask(orientation, facingDirection, position);
    }

    public static void ApplyStonerMask(
        Lemming lemming,
        int frame)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var position = lemming.LevelPosition;

        if (facingDirection == RightFacingDirection.Instance)
        {
            position = orientation.MoveRight(position, 1);
        }

        var key = GetKey(orientation, facingDirection, frame);
        _stonerMasks[key].ApplyEraseMask(orientation, facingDirection, position);
    }

}

public sealed class TerrainMaskTextureReader
{
    private readonly TerrainMask[] _terrainMasks;

    public TerrainMaskTextureReader(
        Texture2D texture,
        IDestructionAction destructionAction,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        _terrainMasks = new TerrainMask[numberOfFrames];

        ReadTerrainMasks(
            texture,
            destructionAction,
            spriteWidth,
            spriteHeight,
            numberOfFrames,
            anchorPoint);
    }

    public TerrainMask TerrainMaskForFrame(int frame)
    {
        return _terrainMasks[frame];
    }

    private void ReadTerrainMasks(
        Texture2D texture,
        IDestructionAction destructionAction,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        var uints = new uint[texture.Width * texture.Height];
        texture.GetData(uints);

        var levelPositions = new List<LevelPosition>();

        for (var f = 0; f < numberOfFrames; f++)
        {
            var y0 = f * spriteHeight;

            for (var x = 0; x < spriteWidth; x++)
            {
                for (var y = 0; y < spriteHeight; y++)
                {
                    var index = x + spriteWidth * (y0 + y);

                    var pixel = uints[index];

                    if (pixel != 0U)
                    {
                        levelPositions.Add(new LevelPosition(x, y));
                    }
                }
            }

            _terrainMasks[f] = new TerrainMask(destructionAction, anchorPoint, levelPositions.ToArray());
            levelPositions.Clear();
        }

        texture.Dispose();
    }
}