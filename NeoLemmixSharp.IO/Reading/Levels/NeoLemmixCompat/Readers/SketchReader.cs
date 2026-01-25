using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class SketchReader : NeoLemmixDataReader
{
    private readonly List<SketchInstanceData> _allSketchData;

    private SketchInstanceData? _currentSketchData;

    private bool _rotate;
    private bool _flipHorizontally;
    private bool _flipVertically;

    public SketchReader(
        List<SketchInstanceData> allSketchData)
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
        _currentSketchData = new SketchInstanceData();
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
        var x = int.Parse(secondToken);
        _currentSketchData!.Position = new Point(x, _currentSketchData.Position.Y);
    }

    private void SetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var y = int.Parse(secondToken);
        _currentSketchData!.Position = new Point(_currentSketchData.Position.X, y);
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
        var dht = new DihedralTransformation(_flipHorizontally, _flipVertically, _rotate);
        _currentSketchData!.Orientation = dht.Orientation;
        _currentSketchData.FacingDirection = dht.FacingDirection;

        _allSketchData.Add(_currentSketchData!);
        _currentSketchData = null;
        FinishedReading = true;
    }
}
