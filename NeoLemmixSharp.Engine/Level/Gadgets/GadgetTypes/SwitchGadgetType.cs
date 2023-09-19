namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SwitchGadgetType : GadgetType
{
    public static SwitchGadgetType Instance { get; } = new();

    private SwitchGadgetType()
    {
    }

    public override int Id => GameConstants.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";
}