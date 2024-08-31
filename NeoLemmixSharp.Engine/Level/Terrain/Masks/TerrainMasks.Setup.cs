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
    public static void InitialiseBasherMask(ReadOnlySpan<byte> byteData)
    {
        InitialiseEraseMask(
            ref _basherMasks,
            BasherAction.Instance,
            byteData,
            new LevelPosition(0, 10));
    }

    public static void InitialiseBomberMask(ReadOnlySpan<byte> byteData)
    {
        InitialiseEraseMask(
            ref _bomberMasks,
            ExploderAction.Instance,
            byteData,
            new LevelPosition(8, 14));
    }

    public static void InitialiseFencerMask(ReadOnlySpan<byte> byteData)
    {
        InitialiseEraseMask(
            ref _fencerMasks,
            FencerAction.Instance,
            byteData,
            new LevelPosition(0, 10));
    }

    public static void InitialiseMinerMask(ReadOnlySpan<byte> byteData)
    {
        InitialiseEraseMask(
            ref _minerMasks,
            MinerAction.Instance,
            byteData,
            new LevelPosition(1, 12));
    }

    private static void InitialiseEraseMask(
        ref TerrainEraseMask[] terrainEraseMasks,
        IDestructionMask destructionMask,
        ReadOnlySpan<byte> byteData,
        LevelPosition anchorPoint)
    {
        if (terrainEraseMasks.Length != 0)
            throw new InvalidOperationException($"Erase mask [{destructionMask.Name}] is already initialised!");

        int maskWidth = byteData[0];
        int maskHeight = byteData[1];
        int numberOfFrames = byteData[2];

        var numberOfTerrainMasks = numberOfFrames *
                                   Orientation.AllItems.Length *
                                   FacingDirection.AllItems.Length;

        terrainEraseMasks = new TerrainEraseMask[numberOfTerrainMasks];

        byteData = byteData[3..];

        var maskDataSpanLength = byteData.Length * 8;

        Span<bool> maskData = stackalloc bool[maskDataSpanLength];

        for (var i = 0; i < byteData.Length; i++)
        {
            var subSpan = maskData.Slice(i * 8, 8);
            var inputByte = byteData[i];

            ExpandByteBits(inputByte, subSpan);
        }

        ;









    }

    private static void ExpandByteBits(byte input, Span<bool> output)
    {
        if (output.Length != 8)
            throw new InvalidOperationException("Expected span of length 8!");

        output[0] = ((input >> 0) & 1) != 0;
        output[1] = ((input >> 1) & 1) != 0;
        output[2] = ((input >> 2) & 1) != 0;
        output[3] = ((input >> 3) & 1) != 0;
        output[4] = ((input >> 4) & 1) != 0;
        output[5] = ((input >> 5) & 1) != 0;
        output[6] = ((input >> 6) & 1) != 0;
        output[7] = ((input >> 7) & 1) != 0;
    }

    private static void ReadTerrainMasks(
        Span<TerrainEraseMask> terrainMasks,
        IDestructionMask destructionMask,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint,
        ReadOnlySpan<bool> mask)
    {
        var levelPositions = new List<LevelPosition>();

        for (var f = 0; f < numberOfFrames; f++)
        {
            var y0 = f * spriteHeight;

            for (var x = 0; x < spriteWidth; x++)
            {
                for (var y = 0; y < spriteHeight; y++)
                {
                    var index = x + spriteWidth * (y0 + y);

                    if (mask[index])
                    {
                        levelPositions.Add(new LevelPosition(x, y));
                    }
                }
            }

            terrainMasks[f] = new TerrainEraseMask(destructionMask, anchorPoint, levelPositions.ToArray());
            levelPositions.Clear();
        }
    }













    public static void InitialiseTerrainMasks(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        var maskCreator = new SpriteRotationReflectionProcessor<TerrainMaskTextureReader>(graphicsDevice);

      /*  _basherMasks = CreateTerrainMaskArray(
            BasherAction.Instance,
            "basher",
            4);

        _bomberMasks = CreateTerrainMaskArray(
            ExploderAction.Instance,
            "bomber",
            1);

        _fencerMasks = CreateTerrainMaskArray(
            FencerAction.Instance,
            "fencer"
            4);*/

        /* _laserMasks = CreateTerrainMaskArray(
             LasererAction.Instance,
             "laser",
             new LevelPosition(3, 10),
             1);*/

      /*  _minerMasks = CreateTerrainMaskArray(
            MinerAction.Instance,
            "miner",
            2);*/

        /* _stonerMasks = CreateTerrainMaskArray(
             StonerAction.Instance,
             "stoner",
             new LevelPosition(16, 25),
             1);*/

        return;

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