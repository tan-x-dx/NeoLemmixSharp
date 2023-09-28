using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    private readonly LevelPosition _spawnPositionTranslation;

    public override GadgetType Type => HatchGadgetType.Instance;
    public override Orientation Orientation { get; }

    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public LevelPosition SpawnPosition => Global.TerrainManager.NormalisePosition(GadgetBounds.TopLeft + _spawnPositionTranslation);

    public HatchGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        Orientation orientation,
        LevelPosition spawnPositionTranslation)
        : base(id, gadgetBounds)
    {
        Orientation = orientation;
        _spawnPositionTranslation = spawnPositionTranslation;

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

    public override IGadgetInput? GetInputWithName(string inputName)
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
        var terrainManager = Global.TerrainManager;

        PreviousTopLeftPixel = terrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = terrainManager.NormalisePosition(BottomRightPixel);

        position = terrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = terrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = terrainManager.NormalisePosition(GadgetBounds.BottomRight);
    }
}