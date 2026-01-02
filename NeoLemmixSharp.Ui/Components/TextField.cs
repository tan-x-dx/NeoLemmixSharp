using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Ui.Components;

public unsafe sealed class TextField : Component
{
    private char* _charBuffer = default;

    public TextField(int x, int y, string label) : base(x, y, label)
    {
    }

    public void SetCapacity(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);

        nint handle = (nint)_charBuffer;
        nint newHandle = handle == 0
            ? Marshal.AllocHGlobal(capacity)
            : Marshal.ReAllocHGlobal(handle, capacity);
        _charBuffer = (char*)newHandle;
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
    }

    protected override void OnDispose()
    {
        nint handle = (nint)_charBuffer;
        if (handle != 0)
            Marshal.FreeHGlobal(handle);
    }
}
