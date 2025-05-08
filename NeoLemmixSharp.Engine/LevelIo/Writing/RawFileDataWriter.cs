using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.Writing;

public sealed class RawFileDataWriter<TPerfectHasher, TEnum>
    where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
    where TEnum : unmanaged, Enum
{
    private const byte Period = (byte)'.';
    private const int InitialDataCapacity = 1 << 11;

    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIntervals = new(new TPerfectHasher());
    private byte[] _mainDataByteBuffer = new byte[InitialDataCapacity];
    private int _mainDataPosition;

    private int _currentSectionStartPosition = -1;
    private TEnum? _currentSectionIdentifier;

    public void WriteToFile(
        string filePath,
        FileVersion version)
    {
        AssertCanWriteToFile();

        var preambleDataByteBuffer = new byte[256];
        var preamblePosition = 0;

        WriteVersion(version, ref preambleDataByteBuffer, ref preamblePosition);
        WriteSectionIntervals(ref preambleDataByteBuffer, ref preamblePosition);

        FileWritingException.WriterAssert(
            _mainDataPosition + preamblePosition <= LevelReadWriteHelpers.MaxAllowedFileSizeInBytes,
            LevelReadWriteHelpers.FileSizeTooLargeExceptionMessage);

        using var fileStream = new FileStream(filePath, FileMode.Create);
        fileStream.Write(new ReadOnlySpan<byte>(preambleDataByteBuffer, 0, preamblePosition));
        fileStream.Write(new ReadOnlySpan<byte>(_mainDataByteBuffer, 0, _mainDataPosition));
    }

    private static void WriteVersion(
        FileVersion version,
        ref byte[] preambleDataByteBuffer,
        ref int preamblePosition)
    {
        WriteToByteBuffer(version.Major, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(version.Minor, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(version.Build, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(version.Revision, ref preambleDataByteBuffer, ref preamblePosition);
    }

    private void WriteSectionIntervals(
        ref byte[] preambleDataByteBuffer,
        ref int preamblePosition)
    {
        WriteToByteBuffer((byte)_sectionIntervals.Count, ref preambleDataByteBuffer, ref preamblePosition);

        // One byte for the section identifier, plus the size of the Interval type
        const int NumberOfBytesPerSectionIdentiferChunk = 1 + 2 * sizeof(int);

        var intervalOffset = preamblePosition + _sectionIntervals.Count * NumberOfBytesPerSectionIdentiferChunk;

        foreach (var kvp in _sectionIntervals)
        {
            WriteSectionIntervalData(
                ref preambleDataByteBuffer,
                ref preamblePosition,
                intervalOffset,
                kvp.Key,
                kvp.Value);
        }
    }

    private static void WriteSectionIntervalData(
        ref byte[] preambleDataByteBuffer,
        ref int preamblePosition,
        int intervalOffset,
        TEnum sectionIdentifier,
        Interval interval)
    {
        var sectionIdentifierByte = (byte)Unsafe.As<TEnum, int>(ref sectionIdentifier);
        WriteToByteBuffer(sectionIdentifierByte, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(interval.Start + intervalOffset, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(interval.Length, ref preambleDataByteBuffer, ref preamblePosition);
    }

    private static unsafe void WriteToByteBuffer<T>(T value, ref byte[] byteBuffer, ref int position)
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        if (position + typeSize >= byteBuffer.Length)
        {
            DoubleByteBufferLength(ref byteBuffer);
        }

        Unsafe.WriteUnaligned(ref byteBuffer[position], value);
        position += typeSize;
    }

    private static void DoubleByteBufferLength(ref byte[] byteBuffer)
    {
        var newBytes = new byte[byteBuffer.Length << 1];
        new ReadOnlySpan<byte>(byteBuffer).CopyTo(newBytes);
        byteBuffer = newBytes;
    }

    public void BeginWritingSection(TEnum sectionIdentifier)
    {
        AssertCanStartNewSection(sectionIdentifier);

        _currentSectionIdentifier = sectionIdentifier;
        _currentSectionStartPosition = _mainDataPosition;
    }

    public void Write(byte value) => WriteSectionData(value);
    public void Write(ushort value) => WriteSectionData(value);
    public void Write(int value) => WriteSectionData(value);
    public void Write(uint value) => WriteSectionData(value);
    public void Write(ulong value) => WriteSectionData(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteSectionData<T>(T value)
        where T : unmanaged
    {
        AssertWithinSection();

        WriteToByteBuffer(value, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        AssertWithinSection();

        if (_mainDataPosition + data.Length >= _mainDataByteBuffer.Length)
        {
            DoubleByteBufferLength(ref _mainDataByteBuffer);
        }

        var destinationSpan = new Span<byte>(_mainDataByteBuffer, _mainDataPosition, data.Length);
        data.CopyTo(destinationSpan);
        _mainDataPosition += data.Length;
    }

    public void EndWritingSection(TEnum sectionIdentifier)
    {
        AssertCanEndSection(sectionIdentifier);

        var interval = new Interval(
            _currentSectionStartPosition,
            _mainDataPosition - _currentSectionStartPosition);
        _sectionIntervals.Add(sectionIdentifier, interval);

        _currentSectionIdentifier = null;
        _currentSectionStartPosition = -1;
    }

    private void AssertCanStartNewSection(TEnum sectionIdentifier)
    {
        FileWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot begin new section while in middle of other section!");
        FileWritingException.WriterAssert(!_sectionIntervals.ContainsKey(sectionIdentifier), "Section has already been written!");
        FileWritingException.WriterAssert(_currentSectionStartPosition < 0, "Invalid section writing state!");
    }

    private void AssertWithinSection()
    {
        FileWritingException.WriterAssert(_currentSectionIdentifier.HasValue, "Cannot write - Not in section!");
    }

    private void AssertCanEndSection(TEnum sectionIdentifier)
    {
        FileWritingException.WriterAssert(_currentSectionIdentifier.HasValue, "Cannot end section - Not in section at all!");

        var v = _currentSectionIdentifier.Value;
        var currentSectionIdentifierValue = Unsafe.As<TEnum, int>(ref v);
        var sectionIdentifierValue = Unsafe.As<TEnum, int>(ref sectionIdentifier);

        FileWritingException.WriterAssert(currentSectionIdentifierValue == sectionIdentifierValue, "Mismatching section start/end!");
        FileWritingException.WriterAssert(_currentSectionStartPosition >= 0, "Invalid section writing state!");
    }

    private void AssertCanWriteToFile()
    {
        FileWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot write to file - In middle of a section!");
        FileWritingException.WriterAssert(_sectionIntervals.Count > 0, "No sections written!");

        new LevelReadWriteHelpers.SectionIdentifierComparer<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(_sectionIntervals);
    }
}
