namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SawBladeGadgetType : GadgetType
{
    public static SawBladeGadgetType Instance { get; } = new();

    private SawBladeGadgetType()
    {
    }

    public override int Id => GameConstants.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";
}