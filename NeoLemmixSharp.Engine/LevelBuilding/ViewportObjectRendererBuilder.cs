using Microsoft.Xna.Framework.Content;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class ViewportObjectRendererBuilder
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
        out GadgetRenderer[] behindTerrainSprites,
        out GadgetRenderer[] inFrontOfTerrainSprites,
        out LemmingRenderer[] lemmingSprites)
    {
        var behindTerrainSpritesList = new List<GadgetRenderer>(_gadgets.Length);
        var inFrontOfTerrainSpritesList = new List<GadgetRenderer>(_gadgets.Length);

        var numberOfLemmingSprites = _lemmings.Length;

        lemmingSprites = new LemmingRenderer[numberOfLemmingSprites];

        numberOfLemmingSprites--;

        while (numberOfLemmingSprites >= 0)
        {
            lemmingSprites.At(numberOfLemmingSprites) = new LemmingRenderer(numberOfLemmingSprites);
            numberOfLemmingSprites--;
        }

        lemmingSprites.Sort(SortLemmingRenderers);

        foreach (var gadget in _gadgets)
        {
            var renderer = gadget.CurrentState.Renderer;

            if (renderer.RenderMode == GadgetRenderMode.BehindTerrain)
            {
                behindTerrainSpritesList.Add(renderer);
            }
            else if (renderer.RenderMode == GadgetRenderMode.InFrontOfTerrain)
            {
                inFrontOfTerrainSpritesList.Add(renderer);
            }
        }

        behindTerrainSprites = behindTerrainSpritesList.ToArray();
        inFrontOfTerrainSprites = inFrontOfTerrainSpritesList.ToArray();
    }

    private static int SortLemmingRenderers(IViewportObjectRenderer x, IViewportObjectRenderer y)
    {
        throw new NotImplementedException();
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(ContentManager contentManager)
    {
        return ControlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(contentManager);
    }
}
