namespace NeoLemmixSharp.IO.LevelReading;

public sealed class TerrainData
{
    public int Id { get; }

    public int X { get; set; }
    public int Y { get; set; }
    public bool NoOverwrite { get; set; }

    public string CurrentParsingPath { get; set; }

    public TerrainData(int id)
    {
        Id = id;
    }
}