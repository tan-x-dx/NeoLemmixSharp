using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelList : Component
{
    private LevelBrowserEntry? _selectedEntry;

    public LevelList()
        : base(0, 0, 0, 0)
    {
        _children = new List<Component>();
        Colors = new ColorPacket(
            new Color(0xff111111),
            new Color(0xff111111),
            new Color(0xff111111),
            new Color(0xff111111));
    }

    public void RefreshLevels(List<LevelBrowserEntry> levelBrowserEntries)
    {
        var children = _children!;
        children.Clear();

        foreach (var levelBrowserEntry in levelBrowserEntries.SelectMany(l => l.GetSubEntries()))
        {
            levelBrowserEntry.Index = children.Count;
            levelBrowserEntry.SetClickAction(OnEntryClick);


            children.Add(levelBrowserEntry);

        }
    }

    private void OnEntryClick()
    {
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);


    }
}
