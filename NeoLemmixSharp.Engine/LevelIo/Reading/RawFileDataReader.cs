using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.LevelIo.Writing;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.Reading;

public sealed class RawFileDataReader<TPerfectHasher, TEnum>
    where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
    where TEnum : unmanaged, Enum
{
    private const byte Period = (byte)'.';

    private readonly byte[] _byteBuffer;
    public FileVersion Version { get; }
    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIdentifiers;

    private int _position;

    public int FileSizeInBytes => _byteBuffer.Length;
    public int Position => _position;
    public bool MoreToRead => Position < FileSizeInBytes;

    public RawFileDataReader(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var fileSizeInBytes = fileStream.Length;

            FileReadingException.ReaderAssert(
                fileSizeInBytes <= LevelReadWriteHelpers.MaxAllowedFileSizeInBytes,
                LevelReadWriteHelpers.FileSizeTooLargeExceptionMessage);

            _byteBuffer = GC.AllocateUninitializedArray<byte>((int)fileSizeInBytes);

            fileStream.ReadExactly(_byteBuffer);
        }

        Version = ReadVersion();
        _sectionIdentifiers = ReadSectionIdentifiers();
    }

    private FileVersion ReadVersion()
    {
        ushort major = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort minor = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort build = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort revision = Read16BitUnsignedInteger();

        return new FileVersion(major, minor, build, revision);

        void AssertNextByteIsPeriod()
        {
            int nextByteValue = Read8BitUnsignedInteger();

            FileReadingException.ReaderAssert(nextByteValue == Period, "Version not in correct format");
        }
    }

    private BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> ReadSectionIdentifiers()
    {
        var hasher = new TPerfectHasher();

        int numberOfSections = Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(numberOfSections > 0, "No sections defined in file!");
        FileReadingException.ReaderAssert(numberOfSections <= hasher.NumberOfItems, "Too many sections defined in file!");

        int i = numberOfSections;
        var result = new BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval>(hasher);

        while (i-- > 0)
        {
            int rawIdentifier = Read8BitUnsignedInteger();
            TEnum enumValue = TPerfectHasher.GetEnumValue(rawIdentifier);
            int sectionStart = Read32BitSignedInteger();
            int sectionLength = Read32BitSignedInteger();

            FileReadingException.ReaderAssert(sectionStart > 0, "Invalid interval start!");
            FileReadingException.ReaderAssert(sectionLength > 0, "Invalid interval length!");

            var interval = new Interval(sectionStart, sectionLength);

            result.Add(enumValue, interval);
        }

        new LevelReadWriteHelpers.SectionIdentifierComparer<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(result);

        return result;
    }

    private unsafe T Read<T>()
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        FileReadingException.ReaderAssert(FileSizeInBytes - Position >= typeSize, "Reached end of file!");

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
        FileReadingException.ReaderAssert(FileSizeInBytes - Position >= bufferSize, "Reached end of file!");

        var sourceSpan = new ReadOnlySpan<byte>(_byteBuffer, _position, bufferSize);
        _position += bufferSize;

        return sourceSpan;
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
        if (!_sectionIdentifiers.TryGetValue(sectionIdentifier, out var interval))
        {
            index = -1;
            return false;
        }

        var bytesToSearch = new ReadOnlySpan<byte>(_byteBuffer, interval.Start, interval.Length);
        index = bytesToSearch.IndexOfAny(SearchValues.Create(bytesToLocate));
        if (index < 0)
            return false;

        index += interval.Start;
        return true;
    }

    public void SetReaderPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, FileSizeInBytes);

        _position = position;
    }

    public bool TryGetSectionInterval(TEnum section, out Interval interval) => _sectionIdentifiers.TryGetValue(section, out interval);
}