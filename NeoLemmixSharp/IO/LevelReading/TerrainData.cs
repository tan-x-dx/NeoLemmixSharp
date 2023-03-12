using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class TerrainData
{
    public int Id { get; }

    public string? GroupId { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public bool NoOverwrite { get; set; }
    public bool FlipVertical { get; set; }
    public bool FlipHorizontal { get; set; }
    public bool Erase { get; set; }
    public bool Rotate { get; set; }

    public string CurrentParsingPath { get; set; }

    public TerrainData(int id)
    {
        Id = id;
    }

    public Texture2D? GroupTexture { get; set; }
}