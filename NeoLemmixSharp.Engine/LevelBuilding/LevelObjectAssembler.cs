using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler
{
    private readonly List<Lemming> _lemmings = new();

    private readonly List<GadgetBase> _gadgets = new();
    private readonly List<IViewportObjectRenderer> _gadgetRenderers = new();

    private readonly GadgetSpriteBankBuilder _gadgetSpriteBankBuilder;

    public LevelObjectAssembler(GraphicsDevice graphicsDevice)
    {
        _gadgetSpriteBankBuilder = new GadgetSpriteBankBuilder(graphicsDevice);
    }

    public void AssembleLevelObjects(
        LevelData levelData,
        ContentManager contentManager)
    {
        SetUpGadgets(contentManager, levelData.AllGadgetData);
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

    public Lemming[] GetLevelLemmings(LevelData levelData)
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
                prototype.InitialLemmingAction)
            {
                LevelPosition = new LevelPosition(prototype.X, prototype.Y)
            };

            lemming.State.SetRawData(prototype.Team, prototype.State);

            _lemmings.Add(lemming);
        }

        return _lemmings.ToArray();
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

            var gadget = gadgetBuilder.BuildGadget(_gadgetSpriteBankBuilder, prototype, lemmingHasher);
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

    public IViewportObjectRenderer[] GetLevelSprites()
    {
        var result = new List<IViewportObjectRenderer>();
        foreach (var lemming in _lemmings)
        {
            result.Add(lemming.Renderer);
        }

        result.AddRange(_gadgetRenderers);

        return result.ToArray();
    }

    public LemmingSpriteBank GetLemmingSpriteBank()
    {
        return DefaultLemmingSpriteBank.DefaultLemmingSprites;
    }

    public GadgetSpriteBank GetGadgetSpriteBank()
    {
        return _gadgetSpriteBankBuilder.BuildGadgetSpriteBank();
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(ContentManager contentManager)
    {
        return ControlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(contentManager);
    }

    private void SetUpGadgets(ContentManager contentManager, ICollection<GadgetData> allGadgetData)
    {
        foreach (var gadgetData in allGadgetData)
        {
            //    _gadgetSpriteBankBuilder.LoadGadgetSprite(gadgetData);
        }
    }
}