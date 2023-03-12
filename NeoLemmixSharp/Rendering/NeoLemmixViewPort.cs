using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine;
using System.Diagnostics;

namespace NeoLemmixSharp.Rendering;

public sealed class NeoLemmixViewPort : ITickable
{
    public Zoom Zoom { get; }

    public NeoLemmixViewPort()
    {
        Zoom = new Zoom();
    }

    public void Tick(MouseState mouseState)
    {
        Zoom.TrackScrollWheel(mouseState);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ITickable.ShouldTick => true;
}