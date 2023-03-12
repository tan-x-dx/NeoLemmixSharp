using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class NxlvReader
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly string _filePath;
    private readonly string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly LevelData _levelData = new();

    private readonly Dictionary<string, Texture2D> _textureCache = new();

    private readonly List<TerrainData> _allTerrainData = new();
    private readonly List<TerrainGroup> _allTerrainGroups = new();

    private TerrainData? _currentTerrainData;
    private TerrainGroup? _currentTerrainGroup;

    private bool ParsingTerrainData => _currentTerrainData != null;
    private bool ParsingTerrainGroup => _currentTerrainGroup != null;
    private bool _settingDataForGroup;

    public NxlvReader(GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        string filePath)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _filePath = filePath;
    }

    public LevelScreen CreateLevelFromFile()
    {
        var lines = File.ReadAllLines(_filePath);
        for (var i = 0; i < lines.Length; i++)
        {
            ProcessLine(lines[i]);
        }

        foreach (var terrainGroup in _allTerrainGroups)
        {
            ProcessTerrainGroupTexture(terrainGroup);
        }

        DrawTerrain(_allTerrainData, _levelData.LevelTexture(_graphicsDevice));

        var adjustedTitle = string.IsNullOrWhiteSpace(_levelData.LevelTitle)
            ? "Untitled"
            : _levelData.LevelTitle;

        return new LevelScreen(adjustedTitle)
        {
            LevelObjects = _levelData.LevelObjects.ToArray(),
            LevelSprites = _levelData.LevelSprites.ToArray(),

            TerrainSprite = _levelData.TerrainSprite,
            Viewport = new NeoLemmixViewPort()
        };
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

        if (false)
        {
            ParseObject(tokens);
            return;
        }

        switch (tokens[0])
        {
            case "TITLE":
                _levelData.LevelTitle = string.Join(' ', tokens[1..]);
                break;

            case "AUTHOR":
                _levelData.LevelAuthor = string.Join(' ', tokens[1..]);
                break;

            case "START_X":
                _levelData.LevelStartX = int.Parse(tokens[1]);
                break;

            case "START_Y":
                _levelData.LevelStartY = int.Parse(tokens[1]);
                break;

            case "THEME":
                //    _levelData.LevelAuthor = string.Join(' ', tokens[1..]);
                break;

            case "BACKGROUND":
                //    _levelData.LevelAuthor = string.Join(' ', tokens[1..]);
                break;

            case "WIDTH":
                _levelData.LevelWidth = int.Parse(tokens[1]);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = int.Parse(tokens[1]);
                break;

            case "$TERRAIN":
                ParseTerrain(tokens);
                break;

            case "$TERRAINGROUP":
                ParseTerrainGroup(tokens);
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
                _currentTerrainGroup = new TerrainGroup(_allTerrainGroups.Count);
                _allTerrainGroups.Add(_currentTerrainGroup);
                _allTerrainData.Add(_currentTerrainGroup.DataPlaceholder);
                break;

            case "NAME":
                _currentTerrainGroup!.GroupId = tokens[1];
                _currentTerrainGroup.DataPlaceholder.GroupId = tokens[1];
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
                _currentTerrainData = new TerrainData(_allTerrainData.Count);
                if (_currentTerrainGroup == null)
                {
                    _allTerrainData.Add(_currentTerrainData);
                }
                else
                {
                    _currentTerrainGroup.TerrainDatas.Add(_currentTerrainData);
                }
                break;

            case "STYLE":
                if (tokens[1][0] == '*')
                {
                    _allTerrainData.Remove(_currentTerrainData!);
                    _settingDataForGroup = true;

                    break;
                }

                _currentTerrainData!.CurrentParsingPath = Path.Combine(_rootDirectory, "styles", tokens[1]);
                break;

            case "PIECE":
                if (_settingDataForGroup)
                {
                    var group = _allTerrainGroups.First(tg => tg.GroupId == tokens[1]);
                    
                    _currentTerrainData = group.DataPlaceholder;
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

    private void ParseObject(string[] tokens)
    {
        if (tokens[0] == "") { }
    }

    private void ProcessTerrainGroupTexture(TerrainGroup terrainGroup)
    {
        var minX = terrainGroup.TerrainDatas.Select(td => td.X).Min();
        var minY = terrainGroup.TerrainDatas.Select(td => td.Y).Min();

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            terrainData.X -= minX;
            terrainData.Y -= minY;
        }

        var maxX = terrainGroup.TerrainDatas.Select(GetMaxX).Max();
        var maxY = terrainGroup.TerrainDatas.Select(GetMaxY).Max();

        var texture = new RenderTarget2D(_graphicsDevice, maxX, maxY);

        DrawTerrain(terrainGroup.TerrainDatas, texture);

        terrainGroup.DataPlaceholder.GroupTexture = texture;
    }

    private int GetMaxX(TerrainData terrainData)
    {
        var texture = GetOrLoadTexture(terrainData.CurrentParsingPath);
        return terrainData.X + texture.Width;
    }

    private int GetMaxY(TerrainData terrainData)
    {
        var texture = GetOrLoadTexture(terrainData.CurrentParsingPath);
        return terrainData.Y + texture.Height;
    }

    private void DrawTerrain(List<TerrainData> terrainDataList, RenderTarget2D renderTarget)
    {
        terrainDataList.Sort(SortTerrainEntries);

        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        foreach (var terrainData in terrainDataList)
        {
            ApplyTerrainPiece(terrainData);
        }

        _spriteBatch.End();
        _graphicsDevice.SetRenderTarget(null);
    }

    private static int SortTerrainEntries(TerrainData x, TerrainData y)
    {
        if (x.NoOverwrite != y.NoOverwrite)
        {
            if (x.NoOverwrite)
                return -1;
            return 1;
        }

        var mult = x.NoOverwrite
            ? -1
            : 1;

        return x.Id.CompareTo(y.Id) * mult;
    }

    private void RenderTerrainGroup(IEnumerable<TerrainData> terrainData)
    {

    }

    private void ApplyTerrainPiece(TerrainData terrainData)
    {
        var texture = terrainData.GroupTexture ?? GetOrLoadTexture(terrainData.CurrentParsingPath);

        var s = SpriteEffects.None;

        if (terrainData.FlipVertical)
        {
            s |= SpriteEffects.FlipVertically;
        }

        if (terrainData.FlipHorizontal)
        {
            s |= SpriteEffects.FlipHorizontally;
        }

        var rotation = terrainData.Rotate
            ? (float)(Math.PI / 2)
            : 0f;

        var color = terrainData.Erase
            ? Color.Transparent
            : Color.White;

        var pos = new Vector2(terrainData.X, terrainData.Y);
        _spriteBatch.Draw(texture, pos, null, color, rotation, Vector2.Zero, Vector2.One, s, 1f);
    }

    private Texture2D GetOrLoadTexture(string filePath)
    {
        if (_textureCache.TryGetValue(filePath, out var result))
            return result;

        result = Texture2D.FromFile(_graphicsDevice, filePath);
        _textureCache.Add(filePath, result);

        return result;
    }
}