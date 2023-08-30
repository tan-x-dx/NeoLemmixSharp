namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.Movement;

public interface IMovementBehaviour
{
    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}