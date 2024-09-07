using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class SketchReader : INeoLemmixDataReader
{
    private readonly List<SketchData> _allSketchData;

    private SketchData? _currentSketchData;

    private bool _rotate;
    private bool _flipHorizontally;
    private bool _flipVertically;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKETCH";

    public SketchReader(List<SketchData> allSketchData)
    {
        _allSketchData = allSketchData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentSketchData = new SketchData();
        _rotate = false;
        _flipHorizontally = false;
        _flipVertically = false;

        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        var currentSketchData = _currentSketchData!;

        switch (firstToken)
        {
            case "INDEX":
                currentSketchData.Index = int.Parse(secondToken);
                break;

            case "PIECE":
                break;

            case "X":
                currentSketchData.X = int.Parse(secondToken);
                break;

            case "Y":
                currentSketchData.Y = int.Parse(secondToken);
                break;

            case "ROTATE":
                _rotate = true;
                break;

            case "FLIP_HORIZONTAL":
                _flipHorizontally = true;
                break;

            case "FLIP_VERTICAL":
                _flipVertically = true;
                break;

            case "$END":
                var (rotNum, flip) = DihedralTransformation.Simplify(_flipHorizontally, _flipVertically, _rotate);
                currentSketchData.RotNum = rotNum;
                currentSketchData.Flip = flip;

                _allSketchData.Add(currentSketchData);
                _currentSketchData = null;
                FinishedReading = true;
                break;
        }

        return false;
    }
}