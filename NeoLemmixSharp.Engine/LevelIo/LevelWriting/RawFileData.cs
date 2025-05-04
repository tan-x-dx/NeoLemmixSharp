using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting;

public sealed class RawFileData
{
    private const byte Period = (byte)'.';
    private const int InitialDataCapacity = 1 << 11;

    private readonly BitArrayDictionary<LevelFileSectionIdentifierHasher, BitBuffer32, LevelFileSectionIdentifier, Interval> _sectionIntervals = new(new LevelFileSectionIdentifierHasher());
    private byte[] _mainDataByteBuffer = new byte[InitialDataCapacity];
    private int _mainDataPosition;

    private int _currentSectionStartPosition = -1;
    private LevelFileSectionIdentifier? _currentSectionIdentifier;

    public void WriteToFile(
        string filePath,
        Version version)
    {
        AssertCanWriteToFile();

        var preambleDataByteBuffer = new byte[256];
        var preamblePosition = 0;

        WriteVersion(version, ref preambleDataByteBuffer, ref preamblePosition);
        WriteSectionIntervals(ref preambleDataByteBuffer, ref preamblePosition);

        LevelWritingException.WriterAssert(
            (_mainDataPosition + preamblePosition) <= LevelReadWriteHelpers.MaxAllowedFileSizeInBytes,
            LevelReadWriteHelpers.FileSizeTooLargeExceptionMessage);

        using var fileStream = new FileStream(filePath, FileMode.Create);
        fileStream.Write(new ReadOnlySpan<byte>(preambleDataByteBuffer, 0, preamblePosition));
        fileStream.Write(new ReadOnlySpan<byte>(_mainDataByteBuffer, 0, _mainDataPosition));
    }

    private static void WriteVersion(
        Version version,
        ref byte[] preambleDataByteBuffer,
        ref int preamblePosition)
    {
        WriteToByteBuffer((ushort)version.Major, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer((ushort)version.Minor, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer((ushort)version.Build, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer(Period, ref preambleDataByteBuffer, ref preamblePosition);
        WriteToByteBuffer((ushort)version.Revision, ref preambleDataByteBuffer, ref preamblePosition);
    }

    private unsafe void WriteSectionIntervals(
        ref byte[] preambleDataByteBuffer,
        ref int preamblePosition)
    {
        WriteToByteBuffer((byte)_sectionIntervals.Count, ref preambleDataByteBuffer, ref preamblePosition);

        // One byte for the section identifier, plus the size of the Interval type
        var numberOfBytesPerSectionIdentiferChunk = 1 + sizeof(Interval);

        var intervalOffset = preamblePosition + _sectionIntervals.Count * numberOfBytesPerSectionIdentiferChunk;

        foreach (var (sectionIdentifier, interval) in _sectionIntervals)
        {
            WriteToByteBuffer((byte)sectionIdentifier, ref preambleDataByteBuffer, ref preamblePosition);
            WriteToByteBuffer(interval.Start + intervalOffset, ref preambleDataByteBuffer, ref preamblePosition);
            WriteToByteBuffer(interval.Length, ref preambleDataByteBuffer, ref preamblePosition);
        }
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

    public void BeginWritingSection(LevelFileSectionIdentifier sectionIdentifier)
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

    [SkipLocalsInit]
    public void WriteArgbColor(Color color)
    {
        ReadOnlySpan<byte> colorBytes = [color.A, color.R, color.G, color.B];

        Write(colorBytes);
    }

    [SkipLocalsInit]
    public void WriteRgbColor(Color color)
    {
        ReadOnlySpan<byte> colorBytes = [color.R, color.G, color.B];

        Write(colorBytes);
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

    public void EndWritingSection(LevelFileSectionIdentifier sectionIdentifier)
    {
        AssertCanEndSection(sectionIdentifier);

        var interval = new Interval(
            _currentSectionStartPosition,
            _mainDataPosition - _currentSectionStartPosition);
        _sectionIntervals.Add(sectionIdentifier, interval);

        _currentSectionIdentifier = null;
        _currentSectionStartPosition = -1;
    }

    private void AssertCanStartNewSection(LevelFileSectionIdentifier sectionIdentifier)
    {
        LevelWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot begin new section while in middle of other section!");
        LevelWritingException.WriterAssert(!_sectionIntervals.ContainsKey(sectionIdentifier), "Section has already been written!");
        LevelWritingException.WriterAssert(_currentSectionStartPosition < 0, "Invalid section writing state!");
    }

    private void AssertWithinSection()
    {
        LevelWritingException.WriterAssert(_currentSectionIdentifier.HasValue, "Cannot write - Not in section!");
    }

    private void AssertCanEndSection(LevelFileSectionIdentifier sectionIdentifier)
    {
        LevelWritingException.WriterAssert(_currentSectionIdentifier.HasValue, "Cannot end section - Not in section at all!");
        LevelWritingException.WriterAssert(_currentSectionIdentifier == sectionIdentifier, "Mismatching section start/end!");
        LevelWritingException.WriterAssert(_currentSectionStartPosition >= 0, "Invalid section writing state!");
    }

    private void AssertCanWriteToFile()
    {
        LevelWritingException.WriterAssert(!_currentSectionIdentifier.HasValue, "Cannot write to file - In middle of a section!");
        LevelWritingException.WriterAssert(_sectionIntervals.Count > 0, "No sections written!");
    }
}
