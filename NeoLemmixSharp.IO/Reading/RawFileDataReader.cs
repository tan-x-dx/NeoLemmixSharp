using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading;

internal interface IRawFileDataReader
{
    bool ReadBool();
    byte Read8BitUnsignedInteger();
    ushort Read16BitUnsignedInteger();
    uint Read32BitUnsignedInteger();
    int Read32BitSignedInteger();
    ulong Read64BitUnsignedInteger();

    ReadOnlySpan<byte> ReadBytes(int bufferSize);
}

internal sealed class RawFileDataReader<TPerfectHasher, TEnum> : IRawFileDataReader, IDisposable
    where TPerfectHasher : struct, IEnumIdentifierHelper<BitBuffer32, TEnum>
    where TEnum : unmanaged, Enum
{
    private readonly BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> _sectionIdentifiers;

    private readonly RawArray _byteBuffer = default;
    private int _position;

    public FileFormatVersion FileFormatVersion { get; }
    public int Position => _position;
    public bool MoreToRead => _position < _byteBuffer.Length;

    public RawFileDataReader(Stream stream)
    {
        long streamLength = stream.Length;

        FileReadingException.ReaderAssert(
            streamLength <= IoConstants.MaxAllowedFileSizeInBytes,
            IoConstants.FileSizeTooLargeExceptionMessage);

        _byteBuffer = new RawArray((int)streamLength);

        stream.ReadExactly(_byteBuffer.AsSpan());

        FileFormatVersion = ReadVersion();
        _sectionIdentifiers = ReadSectionIdentifiers();
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
            FileReadingException.ReaderAssert(nextByteValue == IoConstants.PeriodByte, "Version not in correct format!");
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
            ReadSectionIntervalDatum();

        new SectionIdentifierValidator<TPerfectHasher, TEnum>()
            .AssertSectionsAreContiguous(result);

        return result;

        void ReadSectionIntervalDatum()
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
    }

    public bool ReadBool()
    {
        uint byteValue = Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(byteValue <= 1, "Expected bool byte to have a value of either 0 or 1!");

        return (byteValue & 1) != 0U;
    }

    // Special implementation for individual bytes, since it's simpler to execute and also a lot more common.
    public unsafe byte Read8BitUnsignedInteger()
    {
        FileReadingException.ReaderAssert(_position < _byteBuffer.Length, "Reached end of file!");

        byte* pointer = (byte*)_byteBuffer.Handle + _position;
        _position++;

        return *pointer;
    }

    public ushort Read16BitUnsignedInteger() => ReadUnmanaged<ushort>();
    public uint Read32BitUnsignedInteger() => ReadUnmanaged<uint>();
    public int Read32BitSignedInteger() => ReadUnmanaged<int>();
    public ulong Read64BitUnsignedInteger() => ReadUnmanaged<ulong>();

    private unsafe T ReadUnmanaged<T>()
        where T : unmanaged
    {
        var newPosition = _position + sizeof(T);
        FileReadingException.ReaderAssert(newPosition <= _byteBuffer.Length, "Reached end of file!");

        byte* pointer = (byte*)_byteBuffer.Handle + _position;
        _position = newPosition;

        var mask = sizeof(T) - 1;
        if (((int)pointer & mask) == 0)
        {
            // aligned read
            return *(T*)pointer;
        }
        else
        {
            return Unsafe.ReadUnaligned<T>(pointer);
        }
    }

    public unsafe ReadOnlySpan<byte> ReadBytes(int numberOfBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfBytes);

        var newPosition = _position + numberOfBytes;
        FileReadingException.ReaderAssert(newPosition <= _byteBuffer.Length, "Reached end of file!");

        byte* pointer = (byte*)_byteBuffer.Handle + _position;
        _position = newPosition;

        return new ReadOnlySpan<byte>(pointer, numberOfBytes);
    }

    public void SetReaderPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, _byteBuffer.Length);

        _position = position;
    }

    public bool TryGetSectionInterval(TEnum section, out Interval interval) => _sectionIdentifiers.TryGetValue(section, out interval);

    public void Dispose()
    {
        _byteBuffer.Dispose();
    }
}
