using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Writing;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading;

internal sealed class RawFileDataReader<TPerfectHasher, TEnum>
    where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
    where TEnum : unmanaged, Enum
{
    private readonly byte[] _byteBuffer;
    public FileFormatVersion FileFormatVersion { get; }
    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIdentifiers;

    private int _position;

    public int FileSizeInBytes => _byteBuffer.Length;
    public int Position => _position;
    public bool MoreToRead => Position < FileSizeInBytes;

    public RawFileDataReader(Stream stream)
    {
        _byteBuffer = ReadDataFromFile(stream);

        FileFormatVersion = ReadVersion();
        _sectionIdentifiers = ReadSectionIdentifiers();
    }

    private static byte[] ReadDataFromFile(Stream stream)
    {
        var fileSizeInBytes = stream.Length;

        FileReadingException.ReaderAssert(
            fileSizeInBytes <= ReadWriteHelpers.MaxAllowedFileSizeInBytes,
            ReadWriteHelpers.FileSizeTooLargeExceptionMessage);

        var byteBuffer = new byte[(int)fileSizeInBytes];

        stream.ReadExactly(byteBuffer);

        return byteBuffer;
    }

    private FileFormatVersion ReadVersion()
    {
        ushort major = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort minor = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort build = Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        ushort revision = Read16BitUnsignedInteger();

        return new FileFormatVersion(major, minor, build, revision);

        void AssertNextByteIsPeriod()
        {
            int nextByteValue = Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(nextByteValue == ReadWriteHelpers.Period, "Version not in correct format");
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
            uint rawIdentifier = Read8BitUnsignedInteger();
            TEnum enumValue = TPerfectHasher.GetEnumValue(rawIdentifier);
            int sectionStart = Read32BitSignedInteger();
            int sectionLength = Read32BitSignedInteger();

            FileReadingException.ReaderAssert(sectionStart > 0, "Invalid interval start!");
            FileReadingException.ReaderAssert(sectionLength > 0, "Invalid interval length!");

            var interval = new Interval(sectionStart, sectionLength);

            result.Add(enumValue, interval);
        }

        new ReadWriteHelpers.SectionIdentifierComparer<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(result);

        return result;
    }

    private unsafe T Read<T>()
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        FileReadingException.ReaderAssert(FileSizeInBytes - _position >= typeSize, "Reached end of file!");

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
        FileReadingException.ReaderAssert(FileSizeInBytes - _position >= bufferSize, "Reached end of file!");

        var result = new ReadOnlySpan<byte>(_byteBuffer, _position, bufferSize);
        _position += bufferSize;

        return result;
    }

    public void SetReaderPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, FileSizeInBytes);

        _position = position;
    }

    public bool TryGetSectionInterval(TEnum section, out Interval interval) => _sectionIdentifiers.TryGetValue(section, out interval);
}
