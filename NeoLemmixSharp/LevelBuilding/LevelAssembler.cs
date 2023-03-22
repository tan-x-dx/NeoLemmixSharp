using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelAssembler : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private SpriteBank? _spriteBank;

    private readonly List<Lemming> _lemmings = new();

    public LevelAssembler(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    public void AssembleLevel(LevelData levelData, ThemeData themeData)
    {
        _spriteBank = new SpriteBank(_graphicsDevice, _spriteBatch);

        LoadLemmingSprites(levelData, themeData);

        SetUpTestLemmings();
    }

    private void LoadLemmingSprites(LevelData levelData, ThemeData themeData)
    {
        foreach (var lemmingState in ILemmingSkill.AllLemmingStates)
        {
            var pngFilePath = Path.Combine(themeData.LemmingSpritesFilePath, $"{lemmingState.LemmingSkillName}.png");

            var spriteIdentifier = GetSpriteIdentifier(lemmingState.LemmingSkillName);
            var spriteData = themeData.LemmingSpriteDataLookup[spriteIdentifier];

            var texture = Texture2D.FromFile(_graphicsDevice, pngFilePath);

            _spriteBank!.ProcessLemmingSpriteTexture(lemmingState.LemmingSkillName, spriteData, texture);
        }
    }

    private static string GetSpriteIdentifier(string lemmingStateName)
    {
        return $"${lemmingStateName.ToUpperInvariant()}";
    }

    public SpriteBank GetSpriteBank()
    {
        return _spriteBank!;
    }

    public ITickable[] GetLevelTickables()
    {
        return _lemmings.ToArray<ITickable>();
    }

    public IRenderable[] GetLevelRenderables()
    {
        return _lemmings
            .Select(GetLemmingSprite)
            .ToArray<IRenderable>();
    }

    private LemmingSprite GetLemmingSprite(Lemming lemming)
    {
        return new LemmingSprite(lemming);
    }

    public void Dispose()
    {
        _spriteBank = null;
        _lemmings.Clear();
    }

    private void SetUpTestLemmings()
    {
        var lemming0 = new Lemming
        {
               LevelPosition = new LevelPosition(470, 76),
            //   LevelPosition = new LevelPosition(200, 0),
            //    FacingDirection = LeftFacingDirection.Instance
        };

        var lemming1 = new Lemming
        {
            LevelPosition = new LevelPosition(770, 10),
            // LevelPosition = new LevelPosition(126, 42),
            //     Orientation = UpOrientation.Instance,
                FacingDirection = LeftFacingDirection.Instance
        };

        var lemming2 = new Lemming
        {
            LevelPosition = new LevelPosition(692, 72),
       //     LevelPosition = new LevelPosition(60, 20),
        //    Orientation = LeftOrientation.Instance
        };

        var lemming3 = new Lemming
        {
            LevelPosition = new LevelPosition(612, 42),
        //    LevelPosition = new LevelPosition(145, 134),
            //    Orientation = RightOrientation.Instance
        };

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
    }
}