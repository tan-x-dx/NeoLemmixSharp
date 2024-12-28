using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    private LevelPosition _currentPosition;
    private LevelPosition _previousPosition;

    public bool ContainsPoint(LevelPosition levelPosition) => false;

    public LevelPosition TopLeftPixel => _currentPosition;
    public LevelPosition BottomRightPixel => TopLeftPixel;
    public LevelPosition PreviousTopLeftPixel => _previousPosition;
    public LevelPosition PreviousBottomRightPixel => PreviousTopLeftPixel;

    public EmptyHitBoxRegion(LevelPosition levelPosition)
    {
        _currentPosition = LevelScreen.NormalisePosition(levelPosition);
        _previousPosition = _currentPosition;
    }

    public void Move(int dx, int dy)
    {
        _previousPosition = _currentPosition;
        _currentPosition = LevelScreen.NormalisePosition(_currentPosition + new LevelPosition(dx, dy));
    }

    public void SetPosition(int x, int y)
    {
        _previousPosition = _currentPosition;
        _currentPosition = LevelScreen.NormalisePosition(new LevelPosition(x, y));
    }
}