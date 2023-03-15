using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Reading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelReader : IDisposable
{
    private readonly Dictionary<string, IDataReader> _dataReaders = new();

    private IDataReader? _currentDataReader;

    public LevelData LevelData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();

    public LevelReader()
    {
        AddDataReader(new LevelDataReader(LevelData, false));
        AddDataReader(new LevelDataReader(LevelData, true));
        AddDataReader(new SkillSetReader());
        AddDataReader(new TerrainGroupReader(AllTerrainGroups));
        AddDataReader(new GadgetReader());
        AddDataReader(new TerrainReader(AllTerrainData));
    }

    private void AddDataReader(IDataReader dataReader)
    {
        _dataReaders.Add(dataReader.IdentifierToken, dataReader);
    }

    public void ReadLevel(string levelFilePath)
    {
        var lines = File.ReadAllLines(levelFilePath);

        foreach (var line in lines)
        {
            ProcessLine(line);
        }

        AllTerrainGroups.Sort(SortTerrainGroups);
    }

    private void ProcessLine(string line)
    {
        if (line[0] == '#') // comment line - ignore
            return;

        var tokens = line.Trim().Split(' ', StringSplitOptions.TrimEntries);
        if (tokens.Length == 1 && tokens[0].Length == 0) // blank line
            return;

        if (_currentDataReader != null)
        {
            _currentDataReader.ReadNextLine(tokens);

            if (_currentDataReader.FinishedReading)
            {
                _currentDataReader = null;
            }
            return;
        }

        if (_dataReaders.TryGetValue(tokens[0], out var dataReader))
        {
            _currentDataReader = dataReader;
            _currentDataReader.BeginReading(tokens);

            return;
        }

        throw new InvalidOperationException($"Unknown token: [{tokens[0]}] - line: \"{line}\"");
    }

    private static int SortTerrainGroups(TerrainGroup x, TerrainGroup y)
    {
        var xGroupId = x.GroupId!;
        var yGroupId = y.GroupId!;

        if (x.IsPrimitive)
        {
            if (y.IsPrimitive)
                return string.Compare(xGroupId, yGroupId, StringComparison.Ordinal);
            return -1;
        }

        if (y.IsPrimitive)
            return 1;

        if (x.TerrainDatas.Any(td => td.GroupId == yGroupId))
            return 1;

        if (y.TerrainDatas.Any(td => td.GroupId == xGroupId))
            return -1;

        return string.Compare(xGroupId, yGroupId, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        foreach (var terrainGroup in AllTerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }
}