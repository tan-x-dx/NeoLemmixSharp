using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Movement;

public sealed class BasicMovementBehaviour : IMovementBehaviour
{
    private readonly int _gadgetId;
    private readonly RectangularLevelRegion _position;

    public BasicMovementBehaviour(int gadgetId, RectangularLevelRegion position)
    {
        _gadgetId = gadgetId;
        _position = position;
    }

    public void Move(int dx, int dy)
    {
        _position.X += dx;
        _position.Y += dy;

        LevelScreen.Current.GadgetManager.UpdateGadgetPosition(_gadgetId);
    }

    public void SetPosition(int x, int y)
    {
        _position.X = x;
        _position.Y = y;

        LevelScreen.Current.GadgetManager.UpdateGadgetPosition(_gadgetId);
    }
}