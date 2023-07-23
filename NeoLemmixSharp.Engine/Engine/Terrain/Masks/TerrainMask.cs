using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
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

    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;

    public TerrainMask(LevelPosition anchorPoint, LevelPosition[] mask)
    {
        _anchorPoint = anchorPoint;
        _mask = mask;
    }

    public void ApplyMask(LevelPosition position)
    {
        var offset = position - _anchorPoint;

        for (var i = 0; i < _mask.Length; i++)
        {
            var pixel = _mask[i];

            pixel += offset;

            _terrainManager.ErasePixel(pixel);
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

        _basherMasks = Bar(
            "basher",
            new LevelPosition(2, 2),
            4);

        _bomberMasks = Bar(
            "bomber",
            new LevelPosition(1, 1),
            1);

        _fencerMasks = Bar(
            "fencer",
            new LevelPosition(1, 1),
            4);

        /* _laserMasks = Bar(
             "laser",
             new LevelPosition(1, 1),
             1);*/

        _minerMasks = Bar(
            "bomber",
            new LevelPosition(1, 1),
            2);

        _stonerMasks = Bar(
            "stoner",
            new LevelPosition(1, 1),
            1);

        TerrainMask[] Bar(
            string actionName,
            LevelPosition anchorPoint,
            int numberOfFrames)
        {
            return CreateTerrainMasks(
                contentManager,
                maskCreator,
                actionName,
                anchorPoint,
                numberOfFrames);
        }
    }

    private static TerrainMask[] CreateTerrainMasks(
        ContentManager contentManager,
        MaskCreator maskCreator,
        string actionName,
        LevelPosition anchorPoint,
        int numberOfFrames)
    {
        using var texture = contentManager.Load<Texture2D>($"mask/{actionName}");

        var spriteWidth = texture.Width;
        var spriteHeight = texture.Height / numberOfFrames;

        var terrainMaskTextureReaders = maskCreator.CreateAllSpriteTypes(
            texture,
            spriteWidth,
            spriteHeight,
            numberOfFrames,
            1,
            anchorPoint,
            CreateTerrainMaskFromTexture);

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

    public static void ApplyBasherMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _basherMasks[key].ApplyMask(position);
    }

    public static void ApplyBomberMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _bomberMasks[key].ApplyMask(position);
    }

    public static void ApplyFencerMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _fencerMasks[key].ApplyMask(position);
    }

    public static void ApplyLasererMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _laserMasks[key].ApplyMask(position);
    }

    public static void ApplyMinerMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _minerMasks[key].ApplyMask(position);
    }

    public static void ApplyStonerMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position,
        int frame)
    {
        var key = GetKey(orientation, facingDirection, frame);
        _stonerMasks[key].ApplyMask(position);
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

    private static TerrainMaskTextureReader CreateTerrainMaskFromTexture(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int _,
        LevelPosition anchorPoint)
    {
        return new TerrainMaskTextureReader(texture, spriteWidth, spriteHeight, numberOfFrames, anchorPoint);
    }
}

public sealed class TerrainMaskTextureReader
{
    private readonly TerrainMask[] _terrainMasks;

    public TerrainMaskTextureReader(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        _terrainMasks = new TerrainMask[numberOfFrames];

        ReadTerrainMasks(
            texture,
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
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        var uints = new uint[texture.Width * texture.Height];
        texture.GetData(uints);

        var levelPositions = new List<LevelPosition>();

        var i = 0;
        for (var f = 0; f < numberOfFrames; f++)
        {
            for (var x = 0; x < spriteWidth; x++)
            {
                for (var y = 0; y < spriteHeight; y++)
                {
                    var pixel = uints[i++];

                    if (pixel != 0U)
                    {
                        levelPositions.Add(new LevelPosition(x, y));
                    }
                }
            }

            _terrainMasks[f] = new TerrainMask(anchorPoint, levelPositions.ToArray());
            levelPositions.Clear();
        }

        texture.Dispose();
    }
}