using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

public readonly struct GadgetTriggerData(GadgetTriggerName gadgetTriggerName, GadgetTriggerType gadgetTriggerType, int data1, int data2)
{
    public readonly GadgetTriggerName GadgetTriggerName = gadgetTriggerName;
    public readonly GadgetTriggerType GadgerTriggerType = gadgetTriggerType;
    public readonly int Data1 = data1;
    public readonly int Data2 = data2;
}
