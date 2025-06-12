using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Reading;

internal readonly ref struct GadgetHitBoxCriteriaReader<TRawFileDataReader>(TRawFileDataReader rawFileData)
    where TRawFileDataReader : class, IRawFileDataReader
{
    private readonly TRawFileDataReader _rawFileData = rawFileData;

    public HitBoxCriteriaData ReadHitBoxCriteria()
    {
        var allowedLemmingActionIds = ReadUintSequence(LemmingActionConstants.NumberOfLemmingActions);
        var allowedLemmingStateIds = ReadUintSequence(LemmingStateConstants.NumberOfStates);

        byte allowedLemmingTribeId = ReadAllowedLemmingTribeIds();
        byte allowedLemmingOrientationIds = ReadAllowedLemmingOrientationIds();
        byte allowedFacingDirectionId = ReadAllowedLemmingFacingDirectionId();

        return new HitBoxCriteriaData
        {
            AllowedLemmingActionIds = allowedLemmingActionIds,
            AllowedLemmingStateIds = allowedLemmingStateIds,
            AllowedLemmingTribeIds = allowedLemmingTribeId,
            AllowedLemmingOrientationIds = allowedLemmingOrientationIds,
            AllowedFacingDirectionId = allowedFacingDirectionId
        };
    }

    private uint[] ReadUintSequence(int maxNumberOfBits)
    {
        int numberOfBytesToRead = _rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert((numberOfBytesToRead % sizeof(uint)) == 0, "Expected to read a multiple of 4 bytes!");

        var sourceBytes = _rawFileData.ReadBytes(numberOfBytesToRead);
        var sourceUints = MemoryMarshal.Cast<byte, uint>(sourceBytes);

        if (UintSpanIsTrivial(sourceUints, maxNumberOfBits))
            return [];

        return sourceUints.ToArray();
    }

    private static bool UintSpanIsTrivial(ReadOnlySpan<uint> uints, int maxNumberOfBits)
    {
        var popCount = BitArrayHelpers.GetPopCount(uints);

        FileReadingException.ReaderAssert(popCount <= maxNumberOfBits, "Invalid bit sequence!");

        return popCount == 0 || popCount == maxNumberOfBits;
    }

    private byte ReadAllowedLemmingTribeIds()
    {
        uint rawValue = _rawFileData.Read8BitUnsignedInteger();

        const uint TribeMask = (1 << EngineConstants.MaxNumberOfTribes) - 1;
        rawValue &= TribeMask;

        if (rawValue == 0 || rawValue == TribeMask)
            return 0;

        return (byte)rawValue;
    }

    private byte ReadAllowedLemmingOrientationIds()
    {
        uint rawValue = _rawFileData.Read8BitUnsignedInteger();

        const uint OrientationMask = (1 << EngineConstants.NumberOfOrientations) - 1;
        rawValue &= OrientationMask;

        if (rawValue == 0 || rawValue == OrientationMask)
            return 0;

        return (byte)rawValue;
    }

    private byte ReadAllowedLemmingFacingDirectionId()
    {
        uint rawValue = _rawFileData.Read8BitUnsignedInteger();

        var hasFacingDirectionData = ((rawValue >>> 7) & 1) != 0;

        if (hasFacingDirectionData)
            return (byte)(rawValue & 1);

        return 0;
    }
}
