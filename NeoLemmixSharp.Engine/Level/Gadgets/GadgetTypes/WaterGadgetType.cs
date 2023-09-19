namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class WaterGadgetType : GadgetType
{
    public static WaterGadgetType Instance { get; } = new();

    private WaterGadgetType()
    {
    }

    public override int Id => GameConstants.WaterGadgetTypeId;
    public override string GadgetTypeName => "water";
}