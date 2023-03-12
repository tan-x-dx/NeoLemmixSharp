using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.IO;

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

    private TerrainData? _currentTerrainData;

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
        for (var i = 0; i < lines.Length; i += 1)
        {
            ProcessLine(lines[i]);
        }

        DrawTerrain();

        return new LevelScreen(_levelData.LevelTitle)
        {
            LevelObjects = _levelData.LevelObjects.ToArray(),
            LevelSprites = _levelData.LevelSprites.ToArray(),

            TerrainSprite = _levelData.TerrainSprite,
            Viewport = new NeoLemmixViewPort()//levelData.Viewport,
        };
    }

    private void ProcessLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
            return;

        var tokens = line.Trim().Split(' ', StringSplitOptions.TrimEntries);

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

        var identifierToken = tokens[0];

        switch (identifierToken)
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
        }
    }

    private void ParseTerrain(string[] tokens)
    {
        switch (tokens[0])
        {
            case "$TERRAIN":
                _parseState = ParseState.ParseTerrain;
                _currentTerrainData = new TerrainData(_allTerrainData.Count);
                _allTerrainData.Add(_currentTerrainData);
                break;

            case "STYLE":
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

            case "$END":
                _parseState = ParseState.General;
                _currentTerrainData = null;
                break;
        }
    }

    private void ParseObject(string[] tokens)
    {
        if (tokens[0] == "") { }
    }

    private void DrawTerrain()
    {
        _allTerrainData.Sort(SortTerrainEntries);

        var renderTarget = _levelData.LevelTexture(_graphicsDevice);
        _graphicsDevice.SetRenderTarget(renderTarget);
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

        return x.Id.CompareTo(y.Id);
    }

    private void ApplyTerrainPiece(TerrainData terrainData)
    {
        var texture = GetOrLoadTexture(terrainData.CurrentParsingPath);

        var pos = new Vector2(terrainData.X, terrainData.Y);
        _spriteBatch.Draw(texture, pos, Color.White);
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