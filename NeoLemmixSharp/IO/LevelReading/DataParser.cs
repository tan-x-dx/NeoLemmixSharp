using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class DataParser : IDisposable
{
    private readonly string _rootDirectory;
    private readonly Dictionary<string, string> _levelDataByTokens = new();

    public List<TerrainData> TerrainData { get; } = new();
    public List<TerrainGroup> TerrainGroups { get; } = new();
    public List<object> LevelObjects { get; } = new();

    private TerrainData? _currentTerrainData;
    private TerrainGroup? _currentTerrainGroup;

    private bool ParsingTerrainData => _currentTerrainData != null;
    private bool ParsingTerrainGroup => _currentTerrainGroup != null;
    private bool ParsingLevelObject { get; set; }
    private bool _settingDataForGroup;

    public DataParser(string rootDirectory)
    {
        _rootDirectory = rootDirectory;
    }

    public string GetStringDataByToken(string token)
    {
        if (_levelDataByTokens.TryGetValue(token, out var result))
            return result;

        throw new KeyNotFoundException($"Missing token: {token}");
    }

    public int GetIntDataByToken(string token)
    {
        if (_levelDataByTokens.TryGetValue(token, out var result))
            return int.Parse(result);

        throw new KeyNotFoundException($"Missing token: {token}");
    }

    public void ParseLevel(string[] lines)
    {
        for (var i = 0; i < lines.Length; i++)
        {
            ProcessLine(lines[i]);
        }

        TerrainGroups.Sort(SortTerrainGroups);
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

    private void ProcessLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
            return;

        var tokens = line.Trim().Split(' ', StringSplitOptions.TrimEntries);

        if (ParsingTerrainGroup)
        {
            ParseTerrainGroup(tokens);
            return;
        }

        if (ParsingTerrainData)
        {
            ParseTerrain(tokens);
            return;
        }

        if (ParsingLevelObject)
        {
            ParseLevelObject(tokens);
            return;
        }

        switch (tokens[0])
        {
            case "TITLE":
                _levelDataByTokens.Add("TITLE", string.Join(' ', tokens[1..]));
                break;

            case "AUTHOR":
                _levelDataByTokens.Add("AUTHOR", string.Join(' ', tokens[1..]));
                break;

            case "START_X":
                _levelDataByTokens.Add("START_X", tokens[1]);
                break;

            case "START_Y":
                _levelDataByTokens.Add("START_Y", tokens[1]);
                break;

            case "THEME":
                _levelDataByTokens.Add("THEME", tokens[1]);
                break;

            case "BACKGROUND":
                _levelDataByTokens.Add("BACKGROUND", tokens[1]);
                break;

            case "WIDTH":
                _levelDataByTokens.Add("WIDTH", tokens[1]);
                break;

            case "HEIGHT":
                _levelDataByTokens.Add("HEIGHT", tokens[1]);
                break;

            case "$TERRAIN":
                ParseTerrain(tokens);
                break;

            case "$TERRAINGROUP":
                ParseTerrainGroup(tokens);
                break;

            case "$GADGET":
                ParseLevelObject(tokens);
                break;
        }
    }

    private void ParseTerrainGroup(string[] tokens)
    {
        if (ParsingTerrainData)
        {
            ParseTerrain(tokens);
            return;
        }

        switch (tokens[0])
        {
            case "$TERRAINGROUP":
                _currentTerrainGroup = new TerrainGroup();
                break;

            case "NAME":
                _currentTerrainGroup!.GroupId = tokens[1];
                TerrainGroups.Add(_currentTerrainGroup);
                break;

            case "$TERRAIN":
                ParseTerrain(tokens);
                break;

            case "$END":
                _currentTerrainGroup = null;
                break;
        }
    }

    private void ParseTerrain(string[] tokens)
    {
        switch (tokens[0])
        {
            case "$TERRAIN":
                _currentTerrainData = new TerrainData(TerrainData.Count);

                if (_currentTerrainGroup == null)
                {
                    TerrainData.Add(_currentTerrainData);
                }
                else
                {
                    _currentTerrainGroup.TerrainDatas.Add(_currentTerrainData);
                }

                break;

            case "STYLE":
                if (tokens[1][0] == '*')
                {
                    _settingDataForGroup = true;
                }
                else
                {
                    _currentTerrainData!.CurrentParsingPath = Path.Combine(_rootDirectory, "styles", tokens[1]);
                }

                break;

            case "PIECE":
                if (_settingDataForGroup)
                {
                    _currentTerrainData!.GroupId = tokens[1];
                }
                else
                {
                    _currentTerrainData!.CurrentParsingPath = Path.Combine(_currentTerrainData!.CurrentParsingPath, "terrain", $"{tokens[1]}.png");
                }
                break;

            case "X":
                _currentTerrainData!.X = int.Parse(tokens[1]);
                break;

            case "Y":
                _currentTerrainData!.Y = int.Parse(tokens[1]);
                break;

            case "NO_OVERWRITE":
                _currentTerrainData!.NoOverwrite = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                _currentTerrainData!.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                _currentTerrainData!.FlipHorizontal = true;
                break;

            case "ROTATE":
                _currentTerrainData!.Rotate = true;
                break;

            case "ERASE":
                _currentTerrainData!.Erase = true;
                break;

            case "$END":
                _currentTerrainData = null;
                _settingDataForGroup = false;
                break;

            default:
                ;
                break;
        }
    }

    private void ParseLevelObject(string[] tokens)
    {
        switch (tokens[0])
        {
            case "$GADGET":
                ParsingLevelObject = true;
                break;

            case "$END":
                ParsingLevelObject = false;
                break;

            default:
                ;
                break;
        }
    }

    public void Dispose()
    {
        TerrainData.Clear();
        foreach (var terrainGroup in TerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }
}