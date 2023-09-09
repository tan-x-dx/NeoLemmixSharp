using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainMaskTextureReader
{
    private readonly TerrainEraseMask[] _terrainMasks;

    public TerrainMaskTextureReader(
        Texture2D texture,
        IDestructionMask destructionMask,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorPoint)
    {
        _terrainMasks = new TerrainEraseMask[numberOfFrames];

        ReadTerrainMasks(
            texture,
            destructionMask,
            spriteWidth,
            spriteHeight,
            numberOfFrames,
            anchorPoint);
    }

    public TerrainEraseMask TerrainMaskForFrame(int frame)
    {
        return _terrainMasks[frame];
    }

    private void ReadTerrainMasks(
        Texture2D texture,
        IDestructionMask destructionMask,
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

            _terrainMasks[f] = new TerrainEraseMask(destructionMask, anchorPoint, levelPositions.ToArray());
            levelPositions.Clear();
        }

        texture.Dispose();
    }
}