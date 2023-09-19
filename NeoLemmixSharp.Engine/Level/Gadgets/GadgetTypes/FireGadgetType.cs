namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class FireGadgetType : GadgetType
{
    public static FireGadgetType Instance { get; } = new();

    private FireGadgetType()
    {
    }

    public override int Id => GameConstants.FireGadgetTypeId;
    public override string GadgetTypeName => "fire";
}