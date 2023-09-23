using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class GenericGadgetType : InteractiveGadgetType
{
    public static GenericGadgetType Instance { get; } = new();

    private GenericGadgetType()
    {
    }

    public override int Id => Global.GenericGadgetTypeId;
    public override string GadgetTypeName => "generic";

    public override void InteractWithLemming(Lemming lemming)
    {
    }
}