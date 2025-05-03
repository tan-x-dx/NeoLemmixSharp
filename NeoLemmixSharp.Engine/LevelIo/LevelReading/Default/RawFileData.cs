using Microsoft.Xna.Framework;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public sealed class RawFileData<T>
    where T : unmanaged, Enum
{
    private const long MaxAllowedFileSizeInBytes = 1024 * 1024 * 64;

    private readonly byte[] _byteBuffer;
    public Version Version { get; }
    private readonly Dictionary<T, int> _sectionIndices;

    private int _position;

    public int FileSizeInBytes => _byteBuffer.Length;
    public int Position => _position;
    public bool MoreToRead => Position < FileSizeInBytes;

    public RawFileData(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var fileSizeInBytes = fileStream.Length;

            if (fileSizeInBytes > MaxAllowedFileSizeInBytes)
                throw new InvalidOperationException("File too large! Max file size is 64Mb");

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

            LevelReadingException.ReaderAssert(nextByteValue == '.', "Version not in correct format");
        }
    }

    private Dictionary<T, int>? ReadSectionIndices()
    {
        throw new NotImplementedException();
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
        byte alpha = Read8BitUnsignedInteger();
        byte red = Read8BitUnsignedInteger();
        byte green = Read8BitUnsignedInteger();
        byte blue = Read8BitUnsignedInteger();

        return new Color(red, green, blue, alpha);
    }

    public Color ReadRgbColor()
    {
        byte red = Read8BitUnsignedInteger();
        byte green = Read8BitUnsignedInteger();
        byte blue = Read8BitUnsignedInteger();

        return new Color(red, green, blue, (byte)0xff);
    }

    public bool TryLocateSpan(ReadOnlySpan<byte> bytesToLocate, out int index)
    {
        var bytesToSearch = new ReadOnlySpan<byte>(_byteBuffer);
        index = bytesToSearch.IndexOfAny(SearchValues.Create(bytesToLocate));
        return index >= 0;
    }

    public void SetReaderPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, FileSizeInBytes);

        _position = position;
    }
}