using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.FileFormats;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Writing;

internal interface IRawFileDataWriter
{
    void Write(bool value);
    void Write(byte value);
    void Write(ushort value);
    void Write(int value);
    void Write(uint value);
    void Write(ulong value);
    void Write(ReadOnlySpan<byte> data);
}

internal unsafe sealed class RawFileDataWriter<TPerfectHasher, TEnum> : IRawFileDataWriter, IDisposable
    where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
    where TEnum : unmanaged, Enum
{
    private const int InitialMainDataByteBufferLength = 1 << 12;
    private const int InitialPreambleDataByteBufferLength = 1 << 8;

    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIntervals = new(new TPerfectHasher());

    private IntPtr _mainDataByteBufferHandle = IntPtr.Zero;
    private int _mainDataByteBufferLength;
    private int _mainDataPosition;

    private IntPtr _preambleDataByteBufferHandle = IntPtr.Zero;
    private int _preambleDataByteBufferLength;
    private int _preambleDataPosition;

    private int _currentSectionStartPosition = -1;
    private TEnum? _currentSectionIdentifier;

    public RawFileDataWriter()
    {
        _mainDataByteBufferLength = InitialMainDataByteBufferLength;
        _mainDataByteBufferHandle = Marshal.AllocHGlobal(_mainDataByteBufferLength);
        _mainDataPosition = 0;

        _preambleDataByteBufferLength = InitialPreambleDataByteBufferLength;
        _preambleDataByteBufferHandle = Marshal.AllocHGlobal(_preambleDataByteBufferLength);
        _preambleDataPosition = 0;
    }

    public void BeginWritingSection(TEnum sectionIdentifier)
    {
        AssertCanStartNewSection(sectionIdentifier);

        _currentSectionIdentifier = sectionIdentifier;
        _currentSectionStartPosition = _mainDataPosition;
    }

    public void Write(bool value) => WriteSectionData(value ? (byte)1 : (byte)0);
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

        WriteToByteBuffer(value, ref _mainDataByteBufferHandle, ref _mainDataByteBufferLength, ref _mainDataPosition);
    }

    private static void WriteToByteBuffer<T>(T value, ref IntPtr byteBufferHandle, ref int byteBufferLength, ref int position)
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        if (position + typeSize > byteBufferLength)
        {
            DoubleByteBufferLength(ref byteBufferHandle, ref byteBufferLength);
        }

        byte* pointer = (byte*)byteBufferHandle;
        pointer += position;

        Unsafe.WriteUnaligned(pointer, value);
        position += typeSize;
    }

    private static void DoubleByteBufferLength(ref IntPtr byteBufferHandle, ref int byteBufferLength)
    {
        var newByteBufferLength = byteBufferLength << 1;

        FileWritingException.WriterAssert(newByteBufferLength <= IoConstants.MaxAllowedFileSizeInBytes, IoConstants.FileSizeTooLargeExceptionMessage);

        IntPtr newHandle = Marshal.ReAllocHGlobal(byteBufferHandle, newByteBufferLength);

        byteBufferHandle = newHandle;
        byteBufferLength = newByteBufferLength;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        AssertWithinSection();

        if (_mainDataPosition + data.Length >= _mainDataByteBufferLength)
        {
            DoubleByteBufferLength(ref _mainDataByteBufferHandle, ref _mainDataByteBufferLength);
        }

        byte* pointer = (byte*)_mainDataByteBufferHandle;
        pointer += _mainDataPosition;

        var destinationSpan = new Span<byte>(pointer, data.Length);
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

        TEnum v = _currentSectionIdentifier.Value;
        int currentSectionIdentifierValue = Unsafe.As<TEnum, int>(ref v);
        int sectionIdentifierValue = Unsafe.As<TEnum, int>(ref sectionIdentifier);

        FileWritingException.WriterAssert(currentSectionIdentifierValue == sectionIdentifierValue, "Mismatching section start/end!");
        FileWritingException.WriterAssert(_currentSectionStartPosition >= 0, "Invalid section writing state!");
    }

    public void WriteToFile(
        Stream stream,
        FileFormatVersion version)
    {
        AssertCanWriteToFile();

        WriteVersion(version);
        WriteSectionIntervals();

        FileWritingException.WriterAssert(
            _mainDataPosition + _preambleDataPosition <= IoConstants.MaxAllowedFileSizeInBytes,
            IoConstants.FileSizeTooLargeExceptionMessage);

        stream.Write(new ReadOnlySpan<byte>((void*)_preambleDataByteBufferHandle, _preambleDataPosition));
        stream.Write(new ReadOnlySpan<byte>((void*)_mainDataByteBufferHandle, _mainDataPosition));
    }

    private void AssertCanWriteToFile()
    {
        FileWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot write to file - In middle of a section!");
        FileWritingException.WriterAssert(_sectionIntervals.Count > 0, "No sections written!");

        new ReadWriteHelpers.SectionIdentifierComparer<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(_sectionIntervals);
    }

    private void WriteVersion(
        FileFormatVersion version)
    {
        WriteToByteBuffer(version.Major, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(IoConstants.PeriodByte, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(version.Minor, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(IoConstants.PeriodByte, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(version.Build, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(IoConstants.PeriodByte, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(version.Revision, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
    }

    private void WriteSectionIntervals()
    {
        WriteToByteBuffer((byte)_sectionIntervals.Count, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);

        // One byte for the section identifier, plus the size of the Interval type
        const int NumberOfBytesPerSectionIdentiferChunk = 1 + 2 * sizeof(int);

        var intervalOffset = _preambleDataPosition + _sectionIntervals.Count * NumberOfBytesPerSectionIdentiferChunk;

        foreach (var kvp in _sectionIntervals)
        {
            WriteSectionIntervalDatum(
                intervalOffset,
                kvp.Key,
                kvp.Value);
        }
    }

    private void WriteSectionIntervalDatum(
        int intervalOffset,
        TEnum sectionIdentifier,
        Interval interval)
    {
        byte sectionIdentifierByte = (byte)Unsafe.As<TEnum, int>(ref sectionIdentifier);
        WriteToByteBuffer(sectionIdentifierByte, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(interval.Start + intervalOffset, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
        WriteToByteBuffer(interval.Length, ref _preambleDataByteBufferHandle, ref _preambleDataByteBufferLength, ref _preambleDataPosition);
    }

    public void Dispose()
    {
        if (_mainDataByteBufferHandle != IntPtr.Zero)
            Marshal.FreeHGlobal(_mainDataByteBufferHandle);
        if (_preambleDataByteBufferHandle != IntPtr.Zero)
            Marshal.FreeHGlobal(_preambleDataByteBufferHandle);
    }
}
