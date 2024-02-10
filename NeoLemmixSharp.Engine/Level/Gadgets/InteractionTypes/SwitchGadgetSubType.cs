namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class SwitchGadgetSubType : GadgetSubType
{
    public static readonly SwitchGadgetSubType Instance = new();

    private SwitchGadgetSubType()
    {
    }

    public override int Id => LevelConstants.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";
}