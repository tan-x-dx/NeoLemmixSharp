namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IMoveableGadget
{
    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}