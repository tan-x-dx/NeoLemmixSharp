using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBank : IDisposable
{
    public required Texture2D Panel { get; init; }
    public required Texture2D PanelMinimapRegion { get; init; }
    public required Texture2D PanelIcons { get; init; }
    public required Texture2D PanelSkillSelected { get; init; }
    public required Texture2D PanelSkills { get; init; }

    public void Dispose()
    {
        Panel.Dispose();
        PanelMinimapRegion.Dispose();
        PanelIcons.Dispose();
        PanelSkillSelected.Dispose();
        PanelSkills.Dispose();
    }
}