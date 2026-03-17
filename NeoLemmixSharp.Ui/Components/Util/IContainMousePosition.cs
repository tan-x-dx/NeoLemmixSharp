using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Ui.Components.Util;

public interface IContainMousePosition
{
    bool ContainsPoint(Component c, Point position);

    public static IContainMousePosition RectangularCollisionInstance { get; } = new RectangularCollision();
    public static IContainMousePosition NoCollisionInstance { get; } = new NoCollision();

    private sealed class RectangularCollision : IContainMousePosition
    {
        public bool ContainsPoint(Component c, Point position)
        {
            return position.X >= c.Left &&
                   position.Y >= c.Top &&
                   position.X < c.Right &&
                   position.Y < c.Bottom;
        }
    }

    private sealed class NoCollision : IContainMousePosition
    {
        public bool ContainsPoint(Component c, Point position) => false;
    }
}
