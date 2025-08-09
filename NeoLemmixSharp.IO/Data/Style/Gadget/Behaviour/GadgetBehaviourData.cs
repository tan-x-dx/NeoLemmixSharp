namespace NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

public readonly struct GadgetBehaviourData(GadgetBehaviourType gadgetBehaviourType, int data1, int data2, int data3)
{
    public readonly GadgetBehaviourType GadgerBehaviourType = gadgetBehaviourType;
    public readonly int Data1 = data1;
    public readonly int Data2 = data2;
    public readonly int Data3 = data3;
}
