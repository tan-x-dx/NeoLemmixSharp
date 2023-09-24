using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class HatchGadget : GadgetBase
{
    public override GadgetType Type => HatchGadgetType.Instance;
    public override Orientation Orientation { get; }
    public LevelPosition SpawnPosition { get; }

    public HatchGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        Orientation orientation,
        LevelPosition spawnPosition)
        : base(id, gadgetBounds)
    {
        Orientation = orientation;
        SpawnPosition = spawnPosition;
    }

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        throw new NotImplementedException();
    }
}