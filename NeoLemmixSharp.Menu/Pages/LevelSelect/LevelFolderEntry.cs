using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelFolderEntry : LevelBrowserEntry
{
    private bool _isOpen;
    private readonly string _folderPath;
    private List<LevelBrowserEntry>? _entries;

    public override string DisplayName => _folderPath;

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;

            if (_isOpen)
            {
                LoadSubEntries();
            }
        }
    }

    public LevelFolderEntry(
        string folder,
        int indentationLevel)
        : base(indentationLevel)
    {
        _folderPath = folder;

        MouseDoubleClick.RegisterMouseEvent(OnDoubleClick);
    }

    private void OnDoubleClick(Component _, LevelPosition position)
    {
        IsOpen = !IsOpen;
    }

    public void LoadSubEntries()
    {
        _entries ??= new List<LevelBrowserEntry>();
    }

    public override IEnumerable<LevelBrowserEntry> GetSubEntries()
    {
        if (IsOpen && _entries is not null)
            return _entries.SelectMany(l => l.GetSubEntries()).Prepend(this);

        return [this];
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
        FontBank.MenuFont.RenderText(
            spriteBatch,
            DisplayName,
            Left + ButtonPadding,
            Top + ButtonPadding,
            1,
            EngineConstants.PanelBlue);
    }

    protected override void OnDispose()
    {
        if (_entries is null)
            return;

        foreach (var entry in _entries)
        {
            entry.Dispose();
        }
        _entries.Clear();
    }
}
