using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public static partial class TerrainMasks
{
    public static void InitialiseTerrainMasks(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        var maskCreator = new SpriteRotationReflectionProcessor<TerrainMaskTextureReader>(graphicsDevice);

        _basherMasks = CreateTerrainMaskArray(
            BasherAction.Instance,
            "basher",
            new LevelPosition(0, 10),
            4);

        _bomberMasks = CreateTerrainMaskArray(
            ExploderAction.Instance,
            "bomber",
            new LevelPosition(8, 14),
            1);

        _fencerMasks = CreateTerrainMaskArray(
            FencerAction.Instance,
            "fencer",
            new LevelPosition(0, 10),
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

        TerrainEraseMask[] CreateTerrainMaskArray(
            IDestructionMask destructionAction,
            string actionName,
            LevelPosition anchorPoint,
            int numberOfFrames)
        {
            var pathName = $"mask/{actionName}";

            return CreateTerrainMasks(
                contentManager,
                maskCreator,
                destructionAction,
                pathName,
                anchorPoint,
                numberOfFrames);
        }
    }

    public static TerrainEraseMask[] CreateTerrainMasks(
        ContentManager contentManager,
        SpriteRotationReflectionProcessor<TerrainMaskTextureReader> maskCreator,
        IDestructionMask destructionMask,
        string pathName,
        LevelPosition anchorPoint,
        int numberOfFrames)
    {
        using var texture = contentManager.Load<Texture2D>(pathName);

        var spriteWidth = texture.Width;
        var spriteHeight = texture.Height / numberOfFrames;

        var itemCreator = CreateTerrainMaskFromTexture(destructionMask);

        var terrainMaskTextureReaders = maskCreator.CreateAllSpriteTypes(
            texture,
            spriteWidth,
            spriteHeight,
            numberOfFrames,
            1,
            anchorPoint,
            itemCreator);

        var numberOfTerrainMasks = numberOfFrames *
                                   Orientation.AllItems.Length *
                                   FacingDirection.AllItems.Length;

        var result = new TerrainEraseMask[numberOfTerrainMasks];

        for (var f = 0; f < numberOfFrames; f++)
        {
            foreach (var orientation in Orientation.AllItems)
            {
                var k0 = GetKey(orientation, FacingDirection.RightInstance);
                var k1 = GetKey(orientation, FacingDirection.RightInstance, f);

                var terrainMaskTextureReader = terrainMaskTextureReaders[k0];
                var terrainMaskFrame = terrainMaskTextureReader.TerrainMaskForFrame(f);

                result[k1] = terrainMaskFrame;

                k0 = GetKey(orientation, FacingDirection.LeftInstance);
                k1 = GetKey(orientation, FacingDirection.LeftInstance, f);

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

    private static SpriteRotationReflectionProcessor<TerrainMaskTextureReader>.ItemCreator CreateTerrainMaskFromTexture(IDestructionMask destructionMask)
    {
        // Currying is such fun...
        return (t, w, h, f, p) => new TerrainMaskTextureReader(t, destructionMask, w, h, f, p);
    }
}