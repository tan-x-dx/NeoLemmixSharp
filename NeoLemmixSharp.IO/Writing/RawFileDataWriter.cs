using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Util;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Writing;

internal interface IRawFileDataWriter
{
    void WriteBool(bool value);
    void Write8BitUnsignedInteger(byte value);
    void Write16BitUnsignedInteger(ushort value);
    void Write32BitSignedInteger(int value);
    void Write32BitUnsignedInteger(uint value);
    void Write64BitUnsignedInteger(ulong value);
    void WriteBytes(ReadOnlySpan<byte> data);
}

internal sealed class RawFileDataWriter<TPerfectHasher, TEnum> : IRawFileDataWriter, IDisposable
    where TPerfectHasher : struct, IEnumIdentifierHelper<BitBuffer32, TEnum>
    where TEnum : unmanaged, Enum
{
    private const int InitialMainDataByteBufferLength = 1 << 12;
    private const int InitialPreambleDataByteBufferLength = 1 << 8;

    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIntervals = new(new TPerfectHasher());

    private RawArray _mainDataByteBuffer = default;
    private int _mainDataPosition;

    private RawArray _preambleDataByteBuffer = default;
    private int _preambleDataPosition;

    private int _currentSectionStartPosition = -1;
    private TEnum? _currentSectionIdentifier;

    private bool _isDisposed;

    public int Position => _mainDataPosition;

    public RawFileDataWriter()
    {
        _mainDataByteBuffer = new RawArray(InitialMainDataByteBufferLength);
        _mainDataPosition = 0;

        _preambleDataByteBuffer = new RawArray(InitialPreambleDataByteBufferLength);
        _preambleDataPosition = 0;
    }

    public void BeginWritingSection(TEnum sectionIdentifier)
    {
        AssertCanStartNewSection(sectionIdentifier);

        _currentSectionIdentifier = sectionIdentifier;
        _currentSectionStartPosition = _mainDataPosition;
    }

    public void WriteBool(bool value)
    {
        const byte ZeroByte = 0;
        const byte OneByte = 1;

        AssertWithinSection();

        WriteSingleByte(value ? OneByte : ZeroByte, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    public void Write8BitUnsignedInteger(byte value)
    {
        AssertWithinSection();

        WriteSingleByte(value, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    public void Write16BitUnsignedInteger(ushort value) => WriteUnmanaged(value);
    public void Write32BitSignedInteger(int value) => WriteUnmanaged(value);
    public void Write32BitUnsignedInteger(uint value) => WriteUnmanaged(value);
    public void Write64BitUnsignedInteger(ulong value) => WriteUnmanaged(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteUnmanaged<T>(T value)
        where T : unmanaged
    {
        AssertWithinSection();

        WriteUnmanaged(value, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    // Special implementation for individual bytes, since it's simpler to execute and also a lot more common.
    private static unsafe void WriteSingleByte(byte value, ref RawArray byteBuffer, ref int position)
    {
        Debug.Assert(position <= byteBuffer.Length);

        if (position == byteBuffer.Length)
            DoubleByteBufferLength(ref byteBuffer);

        byte* pointer = (byte*)byteBuffer.Handle + position;
        position++;

        *pointer = value;
    }

    private static unsafe void WriteUnmanaged<T>(T value, ref RawArray byteBuffer, ref int position)
        where T : unmanaged
    {
        var newPosition = position + sizeof(T);
        if (newPosition > byteBuffer.Length)
            DoubleByteBufferLength(ref byteBuffer);

        byte* pointer = (byte*)byteBuffer.Handle + position;
        position = newPosition;

        var mask = sizeof(T) - 1;

        if (((int)pointer & mask) == 0)
        {
            // aligned write
            *(T*)pointer = value;
        }
        else
        {
            Unsafe.WriteUnaligned(pointer, value);
        }
    }

    public unsafe void WriteBytes(ReadOnlySpan<byte> data)
    {
        AssertWithinSection();

        var newPosition = _mainDataPosition + data.Length;
        if (newPosition >= _mainDataByteBuffer.Length)
            DoubleByteBufferLength(ref _mainDataByteBuffer);

        byte* pointer = (byte*)_mainDataByteBuffer.Handle + _mainDataPosition;
        _mainDataPosition = newPosition;

        var destinationSpan = Helpers.CreateSpan<byte>(pointer, data.Length);
        data.CopyTo(destinationSpan);
    }

    public void WriteRgbColor(Color color)
    {
        AssertWithinSection();

        WriteSingleByte(color.R, ref _mainDataByteBuffer, ref _mainDataPosition);
        WriteSingleByte(color.G, ref _mainDataByteBuffer, ref _mainDataPosition);
        WriteSingleByte(color.B, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    public void WriteArgbColor(Color color)
    {
        AssertWithinSection();

        WriteSingleByte(color.A, ref _mainDataByteBuffer, ref _mainDataPosition);
        WriteSingleByte(color.R, ref _mainDataByteBuffer, ref _mainDataPosition);
        WriteSingleByte(color.G, ref _mainDataByteBuffer, ref _mainDataPosition);
        WriteSingleByte(color.B, ref _mainDataByteBuffer, ref _mainDataPosition);
    }

    private static void DoubleByteBufferLength(ref RawArray byteBuffer)
    {
        var newByteBufferLength = byteBuffer.Length << 1;

        FileWritingException.WriterAssert(newByteBufferLength <= IoConstants.MaxAllowedFileSizeInBytes, IoConstants.FileSizeTooLargeExceptionMessage);

        nint newHandle = Marshal.ReAllocHGlobal(byteBuffer.Handle, newByteBufferLength);
        byteBuffer = new RawArray(newHandle, newByteBufferLength);
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

    public unsafe void WriteToFile(
        Stream stream,
        FileFormatVersion version)
    {
        AssertCanWriteToFile();

        WriteVersion(version);
        WriteSectionIntervals();

        FileWritingException.WriterAssert(
            _mainDataPosition + _preambleDataPosition <= IoConstants.MaxAllowedFileSizeInBytes,
            IoConstants.FileSizeTooLargeExceptionMessage);

        Debug.Assert(_preambleDataPosition >= 0);
        Debug.Assert(_preambleDataPosition < _preambleDataByteBuffer.Length);
        stream.Write(Helpers.CreateReadOnlySpan<byte>((void*)_preambleDataByteBuffer.Handle, _preambleDataPosition));

        Debug.Assert(_mainDataPosition >= 0);
        Debug.Assert(_mainDataPosition < _mainDataByteBuffer.Length);
        stream.Write(Helpers.CreateReadOnlySpan<byte>((void*)_mainDataByteBuffer.Handle, _mainDataPosition));
    }

    private void AssertCanWriteToFile()
    {
        FileWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot write to file - In middle of a section!");
        FileWritingException.WriterAssert(_sectionIntervals.Count > 0, "No sections written!");

        new SectionIdentifierValidator<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(_sectionIntervals);
    }

    private void WriteVersion(FileFormatVersion version)
    {
        WriteUnmanaged(version.Major, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteSingleByte(IoConstants.PeriodByte, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteUnmanaged(version.Minor, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteSingleByte(IoConstants.PeriodByte, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteUnmanaged(version.Build, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteSingleByte(IoConstants.PeriodByte, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteUnmanaged(version.Revision, ref _preambleDataByteBuffer, ref _preambleDataPosition);
    }

    private void WriteSectionIntervals()
    {
        WriteSingleByte((byte)_sectionIntervals.Count, ref _preambleDataByteBuffer, ref _preambleDataPosition);

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
        WriteSingleByte(sectionIdentifierByte, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteUnmanaged(interval.Start + intervalOffset, ref _preambleDataByteBuffer, ref _preambleDataPosition);
        WriteUnmanaged(interval.Length, ref _preambleDataByteBuffer, ref _preambleDataPosition);
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            _mainDataByteBuffer.Dispose();
            _preambleDataByteBuffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
