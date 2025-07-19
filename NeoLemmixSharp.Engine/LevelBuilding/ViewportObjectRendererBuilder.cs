using Microsoft.Xna.Framework.Content;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly ref struct ViewportObjectRendererBuilder
{
    private readonly GadgetBase[] _gadgets;
    private readonly Lemming[] _lemmings;

    public ViewportObjectRendererBuilder(
        GadgetBase[] gadgets,
        Lemming[] lemmings)
    {
        _gadgets = gadgets;
        _lemmings = lemmings;
    }

    public void GetLevelSprites(
        out List<IViewportObjectRenderer> behindTerrainSprites,
        out List<IViewportObjectRenderer> inFrontOfTerrainSprites,
        out List<IViewportObjectRenderer> lemmingSprites)
    {
        behindTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Length);
        inFrontOfTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Length);
        lemmingSprites = new List<IViewportObjectRenderer>(_lemmings.Length);

        foreach (var gadget in _gadgets)
        {
            var renderer = gadget.CurrentState.Renderer;

            if (renderer.RenderMode == GadgetRenderMode.BehindTerrain)
            {
                behindTerrainSprites.Add(renderer);
            }
            else if (renderer.RenderMode == GadgetRenderMode.InFrontOfTerrain)
            {
                inFrontOfTerrainSprites.Add(renderer);
            }
        }

        foreach (var lemming in _lemmings)
        {
            lemmingSprites.Add(lemming.Renderer);
        }
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(ContentManager contentManager)
    {
        return ControlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(contentManager);
    }
}
