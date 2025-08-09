namespace NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

public readonly struct GadgetTriggerData(GadgetTriggerType gadgetTriggerType, int data1, int data2)
{
    public readonly GadgetTriggerType GadgerTriggerType = gadgetTriggerType;
    public readonly int Data1 = data1;
    public readonly int Data2 = data2;
}
