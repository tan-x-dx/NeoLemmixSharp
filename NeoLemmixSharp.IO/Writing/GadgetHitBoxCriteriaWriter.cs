using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Writing;

internal readonly ref struct GadgetHitBoxCriteriaWriter<TWriter>(TWriter reader)
    where TWriter : class, IRawFileDataWriter
{
    private readonly TWriter _reader = reader;

    internal void WriteHitBoxCriteria(HitBoxCriteriaData overrideHitBoxCriteriaData)
    {
        /*   WriteUintSequence(overrideHitBoxCriteriaData.AllowedLemmingActionIds);
           WriteUintSequence(overrideHitBoxCriteriaData.AllowedLemmingStateIds);

           _reader.Write8BitUnsignedInteger(overrideHitBoxCriteriaData.AllowedLemmingTribeIds);
           _reader.Write8BitUnsignedInteger(overrideHitBoxCriteriaData.AllowedLemmingOrientationIds);
           _reader.Write8BitUnsignedInteger(overrideHitBoxCriteriaData.AllowedFacingDirectionId);*/
    }

    private void WriteUintSequence(ReadOnlySpan<uint> allowedLemmingStateIds)
    {
        if (allowedLemmingStateIds.Length == 0)
        {
            _reader.Write8BitUnsignedInteger((byte)0);
            return;
        }

        var spanAsBytes = MemoryMarshal.AsBytes(allowedLemmingStateIds);

        Debug.Assert((spanAsBytes.Length % sizeof(uint)) == 0);
        _reader.Write8BitUnsignedInteger((byte)spanAsBytes.Length);
        _reader.WriteBytes(spanAsBytes);
    }
}
