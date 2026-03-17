using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public abstract class LevelBrowserEntry : Component
{
    public const int ButtonPadding = 4;

    protected readonly int _indentationLevel;

    public abstract string DisplayName { get; }
    public int Index { get; set; }

    protected LevelBrowserEntry(
        int indentationLevel)
        : base(0, 0)
    {
        _indentationLevel = indentationLevel;

        MouseEnter.RegisterMouseMoveEvent(SetMouseOver);
        MousePressed.RegisterMousePressEvent(SetMousePress, MouseButtonType.Left);
        MouseReleased.RegisterMousePressEvent(SetMouseOver, MouseButtonType.Left);
        MouseExit.RegisterMouseMoveEvent(SetMouseNormal);
    }

    protected abstract override void OnDispose();

    protected abstract override void RenderComponent(SpriteBatch spriteBatch);

    public abstract IEnumerable<LevelBrowserEntry> GetSubEntries();
}
