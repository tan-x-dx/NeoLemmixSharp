using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public sealed class RawFileData<TPerfectHasher, TBuffer, TEnum> : IComparer<Interval>
    where TPerfectHasher : struct, IPerfectHasher<TEnum>, IBitBufferCreator<TBuffer>, IEnumVerifier<TEnum>
    where TBuffer : struct, IBitBuffer
    where TEnum : unmanaged, Enum
{
    private const byte Period = (byte)'.';

    private readonly byte[] _byteBuffer;
    public Version Version { get; }
    private readonly BitArrayDictionary<TPerfectHasher, TBuffer, TEnum, Interval> _sectionIndices;

    private int _position;

    public int FileSizeInBytes => _byteBuffer.Length;
    public int Position => _position;
    public bool MoreToRead => Position < FileSizeInBytes;

    public RawFileData(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var fileSizeInBytes = fileStream.Length;

            LevelReadingException.ReaderAssert(
                fileSizeInBytes <= LevelReadWriteHelpers.MaxAllowedFileSizeInBytes,
                LevelReadWriteHelpers.FileSizeTooLargeExceptionMessage);

            _byteBuffer = GC.AllocateUninitializedArray<byte>((int)fileSizeInBytes);

            fileStream.ReadExactly(_byteBuffer);
        }

        Version = ReadVersion();
        _sectionIndices = ReadSectionIndices();
    }

    private Version ReadVersion()
    {
        int major = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int minor = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int build = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int revision = Read16BitUnsignedInteger();

        return new Version(major, minor, build, revision);

        void AssertNextByteIsPeriod()
        {
            int nextByteValue = Read8BitUnsignedInteger();

            LevelReadingException.ReaderAssert(nextByteValue == Period, "Version not in correct format");
        }
    }

    private BitArrayDictionary<TPerfectHasher, TBuffer, TEnum, Interval> ReadSectionIndices()
    {
        var hasher = new TPerfectHasher();

        int numberOfSections = Read8BitUnsignedInteger();
        LevelReadingException.ReaderAssert(numberOfSections > 0, "No sections defined in file!");
        LevelReadingException.ReaderAssert(numberOfSections <= hasher.NumberOfItems, "Too many sections defined in file!");

        int i = numberOfSections;
        var result = new BitArrayDictionary<TPerfectHasher, TBuffer, TEnum, Interval>(hasher);

        while (i-- > 0)
        {
            int rawIdentifier = Read8BitUnsignedInteger();
            TEnum enumValue = hasher.GetEnumValue(rawIdentifier);
            int sectionStart = Read32BitSignedInteger();
            int sectionLength = Read32BitSignedInteger();

            LevelReadingException.ReaderAssert(sectionStart > 0, "Invalid interval start!");
            LevelReadingException.ReaderAssert(sectionLength > 0, "Invalid interval length!");

            var interval = new Interval(sectionStart, sectionLength);

            result.Add(enumValue, interval);
        }

        AssertSectionsAreContiguous(result);

        return result;
    }

    [SkipLocalsInit]
    private void AssertSectionsAreContiguous(BitArrayDictionary<TPerfectHasher, TBuffer, TEnum, Interval> result)
    {
        Span<Interval> intervals = stackalloc Interval[result.Count];
        result.CopyValuesTo(intervals);

        intervals.Sort(this);

        for (var i = 0; i < intervals.Length - 1; i++)
        {
            var firstInterval = intervals[i];
            var secondInterval = intervals[i + 1];

            LevelReadingException.ReaderAssert(
                firstInterval.Start + firstInterval.Length == secondInterval.Start,
                "Sections are not contiguous!");
        }
    }

    int IComparer<Interval>.Compare(Interval x, Interval y)
    {
        return x.Start.CompareTo(y.Start);
    }

    private unsafe T Read<T>()
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        LevelReadingException.ReaderAssert(FileSizeInBytes - Position >= typeSize, "Reached end of file!");

        var result = Unsafe.ReadUnaligned<T>(ref _byteBuffer[_position]);
        _position += typeSize;

        return result;
    }

    public byte Read8BitUnsignedInteger() => Read<byte>();
    public ushort Read16BitUnsignedInteger() => Read<ushort>();
    public uint Read32BitUnsignedInteger() => Read<uint>();
    public int Read32BitSignedInteger() => Read<int>();
    public ulong Read64BitUnsignedInteger() => Read<ulong>();

    public ReadOnlySpan<byte> ReadBytes(int bufferSize)
    {
        LevelReadingException.ReaderAssert(FileSizeInBytes - Position >= bufferSize, "Reached end of file!");

        var sourceSpan = new ReadOnlySpan<byte>(_byteBuffer, _position, bufferSize);
        _position += bufferSize;

        return sourceSpan;
    }

    public Color ReadArgbColor()
    {
        var bytes = ReadBytes(4);

        return new Color(bytes[1], bytes[2], bytes[3], bytes[0]);
    }

    public Color ReadRgbColor()
    {
        var bytes = ReadBytes(3);

        return new Color(bytes[0], bytes[1], bytes[2], (byte)0xff);
    }

    public bool TryLocateSpan(ReadOnlySpan<byte> bytesToLocate, out int index)
    {
        var bytesToSearch = new ReadOnlySpan<byte>(_byteBuffer);
        index = bytesToSearch.IndexOfAny(SearchValues.Create(bytesToLocate));
        return index >= 0;
    }

    public bool TryLocateSpanWithinSection(
        TEnum sectionIdentifier,
        ReadOnlySpan<byte> bytesToLocate,
        out int index)
    {
        if (!_sectionIndices.TryGetValue(sectionIdentifier, out var interval))
            throw new InvalidOperationException("Section not present!");

        var bytesToSearch = new ReadOnlySpan<byte>(_byteBuffer, interval.Start, interval.Length);
        index = bytesToSearch.IndexOfAny(SearchValues.Create(bytesToLocate));
        return index >= 0;
    }

    public void SetReaderPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, FileSizeInBytes);

        _position = position;
    }

    public bool TryGetSectionInterval(TEnum section, out Interval interval) => _sectionIndices.TryGetValue(section, out interval);
}