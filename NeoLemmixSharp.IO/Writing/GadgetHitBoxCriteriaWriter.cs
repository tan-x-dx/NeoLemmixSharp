using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Writing;

internal readonly ref struct GadgetHitBoxCriteriaWriter<TWriter>(TWriter rawFileData)
    where TWriter : class, IRawFileDataWriter
{
    private readonly TWriter _rawFileData = rawFileData;

    internal void WriteHitBoxCriteria(HitBoxCriteriaData overrideHitBoxCriteriaData)
    {
        WriteUintSequence(overrideHitBoxCriteriaData.AllowedLemmingActionIds);
        WriteUintSequence(overrideHitBoxCriteriaData.AllowedLemmingStateIds);

        _rawFileData.Write(overrideHitBoxCriteriaData.AllowedLemmingTribeIds);
        _rawFileData.Write(overrideHitBoxCriteriaData.AllowedLemmingOrientationIds);
        _rawFileData.Write(overrideHitBoxCriteriaData.AllowedFacingDirectionId);
    }

    private void WriteUintSequence(ReadOnlySpan<uint> allowedLemmingStateIds)
    {
        if (allowedLemmingStateIds.Length == 0)
        {
            _rawFileData.Write((byte)0);
            return;
        }

        var spanAsBytes = MemoryMarshal.AsBytes(allowedLemmingStateIds);

        Debug.Assert((spanAsBytes.Length % sizeof(uint)) == 0);
        _rawFileData.Write((byte)spanAsBytes.Length);
        _rawFileData.Write(spanAsBytes);
    }
}
