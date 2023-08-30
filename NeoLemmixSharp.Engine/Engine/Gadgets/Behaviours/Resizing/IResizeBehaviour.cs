namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Resizing;

public interface IResizeBehaviour
{
    void Resize(int dw, int dh);
    void SetSize(int w, int h);
}