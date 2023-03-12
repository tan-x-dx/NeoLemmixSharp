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

    private ParseState _parseState = ParseState.General;

    private readonly Dictionary<string, Texture2D> _textureCache = new();

    private readonly List<TerrainData> _allTerrainData = new();
    private readonly List<TerrainGroup> _allTerrainGroups = new();

    private TerrainData? _currentTerrainData;
    private TerrainGroup? _currentTerrainGroup;

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

        DrawTerrain();

        return new LevelScreen(_levelData.LevelTitle)
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

        if (_parseState.HasFlag(ParseState.ParseTerrainGroup))
        {
            ParseTerrainGroup(tokens);
            return;
        }

        if (_parseState == ParseState.ParseTerrain)
        {
            ParseTerrain(tokens);
            return;
        }

        if (_parseState == ParseState.ParseObject)
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
        switch (tokens[0])
        {
            case "$TERRAINGROUP":
                _parseState |= ParseState.ParseTerrainGroup;
                _currentTerrainGroup = new TerrainGroup(_allTerrainGroups.Count);
                _allTerrainGroups.Add(_currentTerrainGroup);
                _allTerrainData.Add(_currentTerrainGroup.DataPlaceholder);
                break;

            case "NAME":
                _currentTerrainGroup!.GroupId = tokens[1];
                break;

            case "$TERRAIN":
                ParseTerrain(tokens);
                break;

            case "$END":
                _parseState ^= ParseState.ParseTerrainGroup;
                _currentTerrainGroup = null;
                break;
        }
    }

    private void ParseTerrain(string[] tokens)
    {
        switch (tokens[0])
        {
            case "$TERRAIN":
                _parseState |= ParseState.ParseTerrain;
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
                    goto case "$END";
                }

                _currentTerrainData!.CurrentParsingPath = Path.Combine(_rootDirectory, "styles", tokens[1]);
                break;

            case "PIECE":
                _currentTerrainData!.CurrentParsingPath = Path.Combine(_currentTerrainData!.CurrentParsingPath, "terrain", $"{tokens[1]}.png");
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
                _parseState ^= ParseState.ParseTerrain;
                _currentTerrainData = null;
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
        var minX = terrainGroup.TerrainDatas.MinBy(td => td.X);
        var minY = terrainGroup.TerrainDatas.MinBy(td => td.Y);
    }

    private void DrawTerrain()
    {
        _allTerrainData.Sort(SortTerrainEntries);

        var renderTarget = _levelData.LevelTexture(_graphicsDevice);
        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        foreach (var terrainData in _allTerrainData)
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