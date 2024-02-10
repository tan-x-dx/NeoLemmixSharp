using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler
{
    private readonly SpriteBatch _spriteBatch;

    private readonly List<GadgetBase> _gadgets = new();
    private readonly List<IViewportObjectRenderer> _gadgetRenderers = new();

    private readonly LemmingSpriteBankBuilder _lemmingSpriteBankBuilder;
    private readonly GadgetSpriteBankBuilder _gadgetSpriteBankBuilder;
    private readonly ControlPanelSpriteBankBuilder _controlPanelSpriteBankBuilder;

    public LevelObjectAssembler(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager,
        SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;

        _lemmingSpriteBankBuilder = new LemmingSpriteBankBuilder();
        _gadgetSpriteBankBuilder = new GadgetSpriteBankBuilder(graphicsDevice, contentManager);
        _controlPanelSpriteBankBuilder = new ControlPanelSpriteBankBuilder(graphicsDevice, contentManager);
    }

    public void AssembleLevelObjects(
        LevelData levelData,
        ContentManager contentManager)
    {
        SetUpGadgets(contentManager, levelData.AllGadgetData);
    }

    public HatchGroup[] GetHatchGroups(LevelData levelData)
    {
        return Array.Empty<HatchGroup>();
    }

    public Lemming[] GetLevelLemmings(LevelData levelData)
    {
        var allLemmingData = CollectionsMarshal.AsSpan(levelData.AllLemmingData);

        var result = new Lemming[allLemmingData.Length];
        var i = 0;

        foreach (var prototype in allLemmingData)
        {
            var lemming = new Lemming(
                i,
                prototype.Orientation,
                prototype.FacingDirection,
                prototype.InitialLemmingAction)
            {
                LevelPosition = new LevelPosition(prototype.X, prototype.Y)
            };

            lemming.State.SetRawData(prototype.Team, prototype.State);

            result[i++] = lemming;
        }

        _lemmings.AddRange(result);

        return result;
    }

    public GadgetBase[] GetLevelGadgets()
    {
        return _gadgets.ToArray();
    }

    private readonly List<Lemming> _lemmings = new();
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

    public ControlPanelSpriteBank GetControlPanelSpriteBank()
    {
        return _controlPanelSpriteBankBuilder.BuildControlPanelSpriteBank();
    }

    private void SetUpGadgets(ContentManager contentManager, ICollection<GadgetData> allGadgetData)
    {
        foreach (var gadgetData in allGadgetData)
        {
            //    _gadgetSpriteBankBuilder.LoadGadgetSprite(gadgetData);
        }
    }
}