using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Ui.Components.Util;

public interface IMouseCollision
{
    bool ContainsPoint(Component c, Point position);

    public static IMouseCollision RectangularCollisionInstance { get; } = new RectangularCollision();
    public static IMouseCollision NoCollisionInstance { get; } = new NoCollision();

    private sealed class RectangularCollision : IMouseCollision
    {
        public bool ContainsPoint(Component c, Point position)
        {
            return position.X >= c.Left &&
                   position.Y >= c.Top &&
                   position.X < c.Right &&
                   position.Y < c.Bottom;
        }
    }

    private sealed class NoCollision : IMouseCollision
    {
        public bool ContainsPoint(Component c, Point position) => false;
    }
}
