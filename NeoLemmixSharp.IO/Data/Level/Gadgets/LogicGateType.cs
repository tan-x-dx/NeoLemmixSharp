using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public enum LogicGateType
{
    AndGate = GadgetType.AndGate - GadgetType.AndGate,
    OrGate = GadgetType.OrGate - GadgetType.AndGate,
    NotGate = GadgetType.NotGate - GadgetType.AndGate,
    XorGate = GadgetType.XorGate - GadgetType.AndGate
}
