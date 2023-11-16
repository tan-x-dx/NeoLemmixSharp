﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class HatchGadget : GadgetBase, IMoveableGadget, IReactiveGadget
{
    private readonly LevelPosition _spawnPositionTranslation;

    public override GadgetSubType SubType => HatchGadgetType.Instance;
    public override Orientation Orientation => HatchSpawnData.Orientation;

    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public LevelPosition SpawnPosition => LevelConstants.TerrainManager.NormalisePosition(GadgetBounds.TopLeft + _spawnPositionTranslation);
    public HatchSpawnData HatchSpawnData { get; }

    public HatchGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        LevelPosition spawnPositionTranslation,
        HatchSpawnData hatchSpawnData)
        : base(id, gadgetBounds)
    {
        _spawnPositionTranslation = spawnPositionTranslation;
        HatchSpawnData = hatchSpawnData;

        var topLeft = GadgetBounds.TopLeft;
        var bottomRight = GadgetBounds.BottomRight;

        TopLeftPixel = topLeft;
        BottomRightPixel = bottomRight;

        PreviousTopLeftPixel = topLeft;
        PreviousBottomRightPixel = bottomRight;
    }

    public override void Tick()
    {
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        return null;
    }

    public void Move(int dx, int dy)
    {
        var newPosition = TopLeftPixel + new LevelPosition(dx, dy);

        UpdatePosition(newPosition);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new LevelPosition(x, y);

        UpdatePosition(newPosition);
    }

    private void UpdatePosition(LevelPosition position)
    {
        var terrainManager = LevelConstants.TerrainManager;

        PreviousTopLeftPixel = terrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = terrainManager.NormalisePosition(BottomRightPixel);

        position = terrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = terrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = terrainManager.NormalisePosition(GadgetBounds.BottomRight);
    }

    public bool CanReleaseLemmings()
    {
        return true;
    }
}