﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler
{
	private readonly SpriteBatch _spriteBatch;

	private readonly List<Lemming> _lemmings = new();
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
		ContentManager contentManager,
		LevelData levelData)
	{
		SetUpTestLemmings();
		SetUpLemmings();
		SetUpGadgets(contentManager, levelData.AllGadgetData);
	}
	/*
    private SawBladeGadget LoadSawBlade(ContentManager contentManager)
    {
        var p = new RectangularLevelRegion(100, 100, 14, 14);
        var sawBladeGadget = new SawBladeGadget(2, p);

        _gadgets.Add(sawBladeGadget);
        _gadgetRenderers.Add(new SawBladeRenderer(sawBladeGadget));

        var numberOfFrames = 4;

        var s = new SpriteRotationReflectionProcessor<SawBladeHitMask>(_graphicsDevice);

        using var texture = contentManager.Load<Texture2D>("sprites/style/common/spinner_mask");

        var spriteWidth = texture.Width;
        var spriteHeight = texture.Height / numberOfFrames;

        var sawBladeHitMasks = new List<SawBladeHitMask>();

        for (var i = 0; i < numberOfFrames; i++)
        {
            var sawBladeHitMask = s.CreateSpriteType(
                texture,
                DownOrientation.Instance,
                RightFacingDirection.Instance,
                spriteWidth,
                spriteHeight,
                numberOfFrames,
                1,
                p.TopLeft,
                (a, b, c, d, e, f) => ItemCreator(a, b, c, d, e, f, i, sawBladeGadget));

            sawBladeHitMasks.Add(sawBladeHitMask);
        }

        sawBladeGadget.SetHitMasks(sawBladeHitMasks.ToArray());

        return sawBladeGadget;
    }

    private static SawBladeHitMask ItemCreator(
            Texture2D texture,
            int spriteWidth,
            int spriteHeight,
            int numberOfFrames,
            int numberOfLayers,
            LevelPosition anchorPoint,
            int frame,
            SawBladeGadget sawBladeGadget)
    {
        var uints = new uint[texture.Width * texture.Height];
        texture.GetData(uints);

        var levelPositions = new List<LevelPosition>();

        var y0 = frame * spriteHeight;

        for (var x = 0; x < spriteWidth; x++)
        {
            for (var y = 0; y < spriteHeight; y++)
            {
                var index = x + spriteWidth * (y0 + y);

                var pixel = uints[index];

                if (pixel != 0U)
                {
                    var p = new LevelPosition(x, y);
                    if (levelPositions.Contains(p))
                    {
                        ;
                    }

                    levelPositions.Add(p);
                }
            }
        }

        texture.Dispose();

        return new SawBladeHitMask(sawBladeGadget, sawBladeGadget.GadgetBounds, levelPositions.ToArray());
    }
    */

	public HatchGroup[] GetHatchGroups()
	{
		return Array.Empty<HatchGroup>();
	}

	public Lemming[] GetLevelLemmings()
	{
		SetUpTestLemmings();

		return _lemmings.ToArray();
	}

	public GadgetBase[] GetLevelGadgets()
	{
		return _gadgets.ToArray();
	}

	public IViewportObjectRenderer[] GetLevelSprites()
	{
		var result = new List<IViewportObjectRenderer>(_lemmings.Count + _gadgetRenderers.Count);
		foreach (var lemming in _lemmings)
		{
			var renderer = new LemmingRenderer(lemming);
			lemming.SetRenderer(renderer);
			result.Add(renderer);
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

	private void SetUpTestLemmings()
	{
		int id = 0;

		var lemmingX = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(160, 0),
		};

		var lemmingA = new Lemming(
			id++,
			orientation: UpOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(126, 42)
		};


		var lemmingB = new Lemming(
			id++,
			orientation: LeftOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(60, 20),
			State =
			{
				IsClimber = true,
				IsPermanentFastForwards = true
			}
		};

		var lemmingE = new Lemming(
			id++,
			orientation: LeftOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(60, 24),
		};

		var lemmingC = new Lemming(
			id++,
			orientation: RightOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(145, 134),
			State =
			{
				IsFloater = true
			}
		};

		var lemmingD = new Lemming(
			id++,
			orientation: LeftOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance,
			currentAction: BuilderAction.Instance)
		{
			LevelPosition = new LevelPosition(232, 130)
		};

		BuilderAction.Instance.TransitionLemmingToAction(lemmingD, false);

		var lemming0 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(2, 152)
		};

		var lemming1 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(6, 152)
		};

		var lemming2 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(10, 152)
		};

		var lemming3 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(14, 152)
		};

		var lemming4 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(18, 152)
		};

		var lemming5 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: LeftFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(22, 152)
		};


		var lemming6 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(80, 40)
		};

		var lemming7 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(90, 40)
		};

		var lemming8 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(298, 152)
		};

		var lemming9 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(302, 152)
		};

		var lemming10 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(306, 152)
		};

		var lemming11 = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance)
		{
			LevelPosition = new LevelPosition(310, 152)
		};

		var miner = new Lemming(
			id++,
			orientation: DownOrientation.Instance,
			facingDirection: RightFacingDirection.Instance,
			currentAction: MinerAction.Instance)
		{
			LevelPosition = new LevelPosition(110, 19)
		};
		MinerAction.Instance.TransitionLemmingToAction(miner, false);

		_lemmings.Add(lemmingX);

		_lemmings.Add(lemming0);
		_lemmings.Add(lemming1);
		_lemmings.Add(lemming2);
		_lemmings.Add(lemming3);
		_lemmings.Add(lemming4);
		_lemmings.Add(lemming5);
		_lemmings.Add(lemming6);
		_lemmings.Add(lemming7);
		_lemmings.Add(lemming8);
		_lemmings.Add(lemming9);
		_lemmings.Add(lemming10);
		_lemmings.Add(lemming11);

		_lemmings.Add(lemmingA);
		_lemmings.Add(lemmingB);
		_lemmings.Add(lemmingC);
		_lemmings.Add(lemmingD);
		_lemmings.Add(lemmingE);

		_lemmings.Add(miner);
	}

	private void SetUpLemmings()
	{

	}

	private void SetUpGadgets(ContentManager contentManager, ICollection<GadgetData> allGadgetData)
	{
		foreach (var gadgetData in allGadgetData)
		{
			//    _gadgetSpriteBankBuilder.LoadGadgetSprite(gadgetData);
		}
	}
}