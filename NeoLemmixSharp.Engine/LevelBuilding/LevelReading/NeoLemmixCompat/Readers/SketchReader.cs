using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class SketchReader : NeoLemmixDataReader
{
    private readonly List<SketchData> _allSketchData;

    private SketchData? _currentSketchData;

    private bool _rotate;
    private bool _flipHorizontally;
    private bool _flipVertically;

    public SketchReader(
        List<SketchData> allSketchData)
        : base("$SKETCH")
    {
        _allSketchData = allSketchData;

        SetNumberOfTokens(8);

        RegisterTokenAction("INDEX", SetIndex);
        RegisterTokenAction("PIECE", SetPiece);
        RegisterTokenAction("X", SetX);
        RegisterTokenAction("Y", SetY);
        RegisterTokenAction("ROTATE", SetRotate);
        RegisterTokenAction("FLIP_HORIZONTAL", SetFlipHorizonal);
        RegisterTokenAction("FLIP_VERTICAL", SetFlipVertical);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentSketchData = new SketchData();
        _rotate = false;
        _flipHorizontally = false;
        _flipVertically = false;

        FinishedReading = false;
        return false;
    }

    private void SetIndex(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentSketchData!.Index = int.Parse(secondToken);
    }

    private void SetPiece(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentSketchData!.X = int.Parse(secondToken);
    }

    private void SetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentSketchData!.Y = int.Parse(secondToken);
    }

    private void SetRotate(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _rotate = true;
    }

    private void SetFlipHorizonal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _flipHorizontally = true;
    }

    private void SetFlipVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _flipVertically = true;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var dht = DihedralTransformation.Simplify(_flipHorizontally, _flipVertically, _rotate);
        _currentSketchData!.Orientation = dht.Orientation;
        _currentSketchData.FacingDirection = dht.FacingDirection;

        _allSketchData.Add(_currentSketchData!);
        _currentSketchData = null;
        FinishedReading = true;
    }
}