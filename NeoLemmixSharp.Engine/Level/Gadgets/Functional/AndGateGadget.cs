using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class AndGateGadget : GadgetBase
{
    public override GadgetType Type => GadgetType.Logic;
    public override Orientation Orientation => DownOrientation.Instance;

    public AndGateGadget(int id, RectangularLevelRegion gadgetBounds)
        : base(id, gadgetBounds)
    {
    }

    public override void Tick()
    {

    }

    public override void ReactToInput(string inputName, int payload)
    {
        throw new NotImplementedException();
    }

    public override bool CaresAboutLemmingInteraction => false;

    public override bool MatchesLemming(Lemming lemming) => false;
    public override bool MatchesPosition(LevelPosition levelPosition) => false;
}