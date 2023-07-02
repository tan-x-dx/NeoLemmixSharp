using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Io.LevelReading.Data;
using NeoLemmixSharp.Io.LevelReading.Sprites;

namespace NeoLemmixSharp.Io.LevelReading;

public sealed class TerrainPainter : IDisposable
{
    public const uint MinimumSubstantialAlphaValue = 31;

    private const string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly GraphicsDevice _graphicsDevice;
    private readonly Dictionary<string, PixelColourData> _textureBundleCache = new();

    private readonly List<TerrainGroup> _terrainGroups = new();

    private bool _disposed;

    private Texture2D _terrainTexture;
    private PixelType[] _terrainPixels;

    public TerrainPainter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void PaintLevel(LevelData levelData)
    {
        _terrainGroups.AddRange(levelData.AllTerrainGroups);

        foreach (var terrainGroup in _terrainGroups)
        {
            ProcessTerrainGroup(terrainGroup);
        }

        _terrainTexture = new Texture2D(
            _graphicsDevice,
            levelData.LevelWidth,
            levelData.LevelHeight);

        _terrainPixels = new PixelType[levelData.LevelWidth * levelData.LevelHeight];

        var uintData = new uint[levelData.LevelWidth * levelData.LevelHeight];
        var textureData = new PixelColourData(
            levelData.LevelWidth,
            levelData.LevelHeight,
            uintData);

        DrawTerrain(levelData.AllTerrainData, textureData);
        _terrainTexture.SetData(uintData);
    }

    private static void ProcessTerrainGroup(TerrainGroup terrainGroup)
    {
        var minX = terrainGroup.TerrainDatas.Select(td => td.X).Min();
        var minY = terrainGroup.TerrainDatas.Select(td => td.Y).Min();

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            terrainData.X -= minX;
            terrainData.Y -= minY;
        }
    }

    private void DrawTerrain(
        IEnumerable<TerrainData> terrainDataList,
        PixelColourData targetData,
        int dx = 0,
        int dy = 0)
    {
        foreach (var terrainData in terrainDataList)
        {
            if (terrainData.GroupId == null)
            {
                ApplyTerrainPiece(
                    terrainData,
                    targetData,
                    dx,
                    dy);
            }
            else
            {
                var textureGroup = _terrainGroups.First(tg => tg.GroupId == terrainData.GroupId);
                DrawTerrain(
                    textureGroup.TerrainDatas,
                    targetData,
                    dx + terrainData.X,
                    dy + terrainData.Y);
            }
        }
    }

    private void ApplyTerrainPiece(
        TerrainData terrainData,
        PixelColourData targetPixelColourData,
        int dx,
        int dy)
    {
        var sourcePixelColourData = GetOrLoadPixelColourData(terrainData);

        var dihedralTransformation = DihedralTransformation.GetForTransformation(
            terrainData.FlipHorizontal,
            terrainData.FlipVertical,
            terrainData.Rotate);

        for (var x = 0; x < sourcePixelColourData.Width; x++)
        {
            for (var y = 0; y < sourcePixelColourData.Height; y++)
            {
                dihedralTransformation.Transform(x,
                    y,
                    sourcePixelColourData.Width - 1,
                    sourcePixelColourData.Height - 1, out var x0, out var y0);

                x0 = x0 + terrainData.X + dx;
                y0 = y0 + terrainData.Y + dy;

                if (x0 < 0 || x0 >= targetPixelColourData.Width ||
                    y0 < 0 || y0 >= targetPixelColourData.Height)
                    continue;

                var sourcePixelColour = sourcePixelColourData.Get(x, y);

                if (sourcePixelColour != 0U && terrainData.Tint.HasValue)
                {
                    sourcePixelColour = BlendColours(terrainData.Tint.Value, sourcePixelColour);
                }

                var targetPixelColour = targetPixelColourData.Get(x0, y0);

                if (terrainData.Erase)
                {
                    if (sourcePixelColour != 0U)
                    {
                        targetPixelColour = 0U;
                    }
                }
                else if (terrainData.NoOverwrite)
                {
                    targetPixelColour = BlendColours(targetPixelColour, sourcePixelColour);
                }
                else
                {
                    targetPixelColour = BlendColours(sourcePixelColour, targetPixelColour);
                }

                targetPixelColourData.Set(x0, y0, targetPixelColour);
                var pixelIndex = targetPixelColourData.Width * y0 + x0;
                ref var targetPixelData = ref _terrainPixels[pixelIndex];

                if (PixelColourIsSubstantial(targetPixelColour))
                {
                    targetPixelData = terrainData.IsSteel
                        ? PixelType.Steel
                        : PixelType.Solid;
                }
                else
                {
                    targetPixelData = PixelType.Empty;
                }
            }
        }
    }

    private static uint BlendColours(uint foreground, uint background)
    {
        var fgA = ((foreground >> 24) & 0xffU) / 255d;
        var fgR = ((foreground >> 16) & 0xffU) / 255d;
        var fgG = ((foreground >> 8) & 0xffU) / 255d;
        var fgB = (foreground & 0xffU) / 255d;
        var bgA = ((background >> 24) & 0xffU) / 255d;
        var bgR = ((background >> 16) & 0xffU) / 255d;
        var bgG = ((background >> 8) & 0xffU) / 255d;
        var bgB = (background & 0xffU) / 255d;
        var newA = 1.0 - (1.0 - fgA) * (1.0 - bgA);
        var newR = fgR * fgA / newA + bgR * bgA * (1.0 - fgA) / newA;
        var newG = fgG * fgA / newA + bgG * bgA * (1.0 - fgA) / newA;
        var newB = fgB * fgA / newA + bgB * bgA * (1.0 - fgA) / newA;
        var a = (uint)Math.Round(newA * 255);
        var r = (uint)Math.Round(newR * 255);
        var g = (uint)Math.Round(newG * 255);
        var b = (uint)Math.Round(newB * 255);

        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    private static bool PixelColourIsSubstantial(uint colour)
    {
        var alpha = (colour >> 24) & 0xffU;
        return alpha > MinimumSubstantialAlphaValue;
    }

    private PixelColourData GetOrLoadPixelColourData(TerrainData terrainData)
    {
        var rootFilePath = Path.Combine(_rootDirectory, "styles", terrainData.Style!, "terrain", terrainData.TerrainName!);

        if (_textureBundleCache.TryGetValue(rootFilePath, out var result))
            return result;

        var png = Path.ChangeExtension(rootFilePath, "png");
        var isSteel = File.Exists(Path.ChangeExtension(rootFilePath, "nxmt"));
        terrainData.IsSteel = isSteel;

        using var mainTexture = Texture2D.FromFile(_graphicsDevice, png);
        result = PixelColourData.GetPixelColourDataFromTexture(mainTexture);
        _textureBundleCache.Add(rootFilePath, result);

        return result;
    }

    public PixelType[] GetPixelData()
    {
        return _terrainPixels;
    }

    public Texture2D GetTerrainTexture()
    {
        return _terrainTexture;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _textureBundleCache.Clear();
        _terrainGroups.Clear();
        _disposed = true;
    }
}