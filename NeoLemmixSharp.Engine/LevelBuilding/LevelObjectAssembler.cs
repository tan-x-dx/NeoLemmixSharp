using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler : IDisposable
{
    private readonly List<Lemming> _lemmings = new();
    private readonly List<GadgetBase> _gadgets = new();
    private readonly GadgetSpriteBuilder _gadgetSpriteBuilder;

    public LevelObjectAssembler(GraphicsDevice graphicsDevice)
    {
        _gadgetSpriteBuilder = new GadgetSpriteBuilder(graphicsDevice);
    }

    public static HatchGroup[] GetHatchGroups(LevelData levelData)
    {
        var allHatchGroupData = CollectionsMarshal.AsSpan(levelData.AllHatchGroupData);

        var result = new HatchGroup[allHatchGroupData.Length];
        var i = 0;

        foreach (var prototype in allHatchGroupData)
        {
            var hatchGroup = new HatchGroup(
                i,
                prototype.MinSpawnInterval,
                prototype.MaxSpawnInterval,
                prototype.InitialSpawnInterval);

            result[i++] = hatchGroup;
        }

        return result;
    }

    public ICollection<Lemming> GetLevelLemmings(LevelData levelData)
    {
        var allLemmingData = CollectionsMarshal.AsSpan(levelData.AllLemmingData);

        _lemmings.Capacity = allLemmingData.Length;

        var i = 0;
        foreach (var prototype in allLemmingData)
        {
            var lemming = new Lemming(
                i++,
                prototype.Orientation,
                prototype.FacingDirection,
                prototype.InitialLemmingAction,
                prototype.Team)
            {
                LevelPosition = new LevelPosition(prototype.X, prototype.Y)
            };

            lemming.State.SetRawDataFromOther(prototype.State);

            _lemmings.Add(lemming);
        }

        return _lemmings;
    }

    public GadgetBase[] GetLevelGadgets(
        LevelData levelData,
        IPerfectHasher<Lemming> lemmingHasher)
    {
        var allGadgetData = CollectionsMarshal.AsSpan(levelData.AllGadgetData);

        _gadgets.Capacity = allGadgetData.Length;

        foreach (var prototype in allGadgetData)
        {
            var gadgetBuilder = levelData.AllGadgetBuilders[prototype.GadgetBuilderId];

            var gadget = gadgetBuilder.BuildGadget(_gadgetSpriteBuilder, prototype, lemmingHasher);
            _gadgets.Add(gadget);
        }

        return _gadgets.ToArray();
    }

    public void SetHatchesForHatchGroup(HatchGroup hatchGroup)
    {
        var hatches = new List<HatchGadget>();

        var gadgetSpan = CollectionsMarshal.AsSpan(_gadgets);
        foreach (var gadget in gadgetSpan)
        {
            if (gadget is HatchGadget hatch &&
                hatch.HatchSpawnData.HatchGroupId == hatchGroup.Id)
            {
                hatches.Add(hatch);
            }
        }

        hatchGroup.SetHatches(hatches.ToArray());
    }

    public void GetLevelSprites(
        out ICollection<IViewportObjectRenderer> behindTerrainSprites,
        out ICollection<IViewportObjectRenderer> inFrontOfTerrainSprites,
        out ICollection<IViewportObjectRenderer> lemmingSprites)
    {
        behindTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Count);
        inFrontOfTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Count);
        lemmingSprites = new List<IViewportObjectRenderer>(_lemmings.Count);

        var gadgetSpan = CollectionsMarshal.AsSpan(_gadgets);
        foreach (var gadget in gadgetSpan)
        {
            var renderer = gadget.Renderer;
            if (renderer is null)
                continue;

            if (renderer.RenderMode == GadgetRenderMode.BehindTerrain)
            {
                behindTerrainSprites.Add(renderer);
            }
            else
            {
                inFrontOfTerrainSprites.Add(renderer);
            }
        }

        var lemmingSpan = CollectionsMarshal.AsSpan(_lemmings);
        foreach (var lemming in lemmingSpan)
        {
            lemmingSprites.Add(lemming.Renderer);
        }
    }

    public LemmingSpriteBank GetLemmingSpriteBank()
    {
        return DefaultLemmingSpriteBank.DefaultLemmingSprites;
    }

    public GadgetSpriteBank GetGadgetSpriteBank()
    {
        return _gadgetSpriteBuilder.BuildGadgetSpriteBank();
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(ContentManager contentManager)
    {
        return ControlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(contentManager);
    }

    public void Dispose()
    {
        _lemmings.Clear();
        _gadgets.Clear();
        _gadgetSpriteBuilder.Dispose();
    }
}