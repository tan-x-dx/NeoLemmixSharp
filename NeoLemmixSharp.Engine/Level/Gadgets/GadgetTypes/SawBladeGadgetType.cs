using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SawBladeGadgetType : InteractiveGadgetType
{
    public static SawBladeGadgetType Instance { get; } = new();

    private SawBladeGadgetType()
    {
    }

    public override int Id => Global.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";

    public override void InteractWithLemming(Lemming lemming)
    {

    }
}