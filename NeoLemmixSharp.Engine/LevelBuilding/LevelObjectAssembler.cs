using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler : IDisposable
{
    private readonly LevelData _levelData;

    private readonly List<Lemming> _lemmings = [];
    private readonly List<GadgetBase> _gadgets = [];
    private readonly GadgetRendererBuilder _gadgetSpriteBuilder;
    private readonly Dictionary<StylePiecePair, GadgetArchetypeData> _gadgetArchetypeDataLookup;

    public LevelObjectAssembler(GraphicsDevice graphicsDevice, LevelData levelData)
    {
        _levelData = levelData;
        _gadgetSpriteBuilder = new GadgetRendererBuilder(graphicsDevice);
        _gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(levelData);
    }

    public HatchGroup[] GetHatchGroups()
    {
        var allHatchGroupData = CollectionsMarshal.AsSpan(_levelData.AllHatchGroupData);

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

    public Lemming[] GetLevelLemmings()
    {
        var maxNumberOfClonedLemmings = _levelData.MaxNumberOfClonedLemmings;
        var totalCapacity = maxNumberOfClonedLemmings +
                            _levelData.HatchLemmingData.Count +
                            _levelData.PrePlacedLemmingData.Count;

        _lemmings.Capacity = totalCapacity;

        var prePlacedLemmings = CollectionsMarshal.AsSpan(_levelData.PrePlacedLemmingData);
        var hatchLemmingData = CollectionsMarshal.AsSpan(_levelData.HatchLemmingData);
        AddLemmings(prePlacedLemmings);
        AddLemmings(hatchLemmingData);

        for (var i = 0; i < maxNumberOfClonedLemmings; i++)
        {
            var lemming = new Lemming(
                _lemmings.Count,
                Orientation.Down,
                FacingDirection.Right,
                EngineConstants.NoneActionId,
                EngineConstants.ClassicTeamId)
            {
                AnchorPosition = new Point()
            };
            _lemmings.Add(lemming);
        }

        return _lemmings.ToArray();

        void AddLemmings(ReadOnlySpan<LemmingData> lemmingDataSpan)
        {
            foreach (var prototype in lemmingDataSpan)
            {
                var lemming = new Lemming(
                    _lemmings.Count,
                    prototype.Orientation,
                    prototype.FacingDirection,
                    prototype.InitialLemmingActionId,
                    prototype.TeamId)
                {
                    AnchorPosition = prototype.Position
                };

                lemming.State.SetRawDataFromOther(prototype.State);

                _lemmings.Add(lemming);
            }
        }
    }

    public GadgetBase[] GetLevelGadgets(
        LemmingManager lemmingHasher,
        TeamManager teamManager)
    {
        var allGadgetData = CollectionsMarshal.AsSpan(_levelData.AllGadgetData);

        _gadgets.Capacity = allGadgetData.Length;

        foreach (var prototype in allGadgetData)
        {
            var gadgetArchetypeData = _gadgetArchetypeDataLookup[prototype.GetStylePiecePair()];

            var gadget = GadgetBuilder.BuildGadget(_gadgetSpriteBuilder, gadgetArchetypeData, prototype, lemmingHasher, teamManager);
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
        out List<IViewportObjectRenderer> behindTerrainSprites,
        out List<IViewportObjectRenderer> inFrontOfTerrainSprites,
        out List<IViewportObjectRenderer> lemmingSprites)
    {
        behindTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Count);
        inFrontOfTerrainSprites = new List<IViewportObjectRenderer>(_gadgets.Count);
        lemmingSprites = new List<IViewportObjectRenderer>(_lemmings.Count);

        var gadgetSpan = CollectionsMarshal.AsSpan(_gadgets);
        foreach (var gadget in gadgetSpan)
        {
            var renderer = gadget.Renderer;

            if (renderer.RenderMode == GadgetRenderMode.BehindTerrain)
            {
                behindTerrainSprites.Add(renderer);
            }
            else if (renderer.RenderMode == GadgetRenderMode.InFrontOfTerrain)
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