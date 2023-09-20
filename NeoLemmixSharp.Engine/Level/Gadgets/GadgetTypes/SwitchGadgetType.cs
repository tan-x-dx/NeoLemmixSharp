using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SwitchGadgetType : InteractiveGadgetType
{
    public static SwitchGadgetType Instance { get; } = new();

    private SwitchGadgetType()
    {
    }

    public override int Id => GameConstants.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";

    public override void InteractWithLemming(Lemming lemming)
    {
    }
}