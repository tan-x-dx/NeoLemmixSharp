using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

public readonly struct GadgetTriggerData(GadgetTriggerName gadgetTriggerName, GadgetTriggerType gadgetTriggerType, DataChunk2 dataChunk)
{
    public readonly GadgetTriggerName GadgetTriggerName = gadgetTriggerName;
    public readonly GadgetTriggerType GadgerTriggerType = gadgetTriggerType;
    public readonly DataChunk2 DataChunk = dataChunk;
}
