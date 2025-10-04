using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

public readonly struct GadgetBehaviourData(GadgetBehaviourName gadgetBehaviourName, GadgetBehaviourType gadgetBehaviourType, DataChunk3 dataChunk)
{
    public readonly GadgetBehaviourName GadgetBehaviourName = gadgetBehaviourName;
    public readonly GadgetBehaviourType GadgetBehaviourType = gadgetBehaviourType;
    public readonly DataChunk3 DataChunk = dataChunk;
}
