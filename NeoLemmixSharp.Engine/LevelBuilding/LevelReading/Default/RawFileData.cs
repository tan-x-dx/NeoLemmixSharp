using Microsoft.Xna.Framework;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class RawFileData
{
    private const long MaxAllowedFileSizeInBytes = 1024 * 1024 * 64;

    private readonly byte[] _byteBuffer;
    private int _position;
    public Version Version { get; }

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

            LevelReadWriteHelpers.ReaderAssert(nextByteValue == '.', "Version not in correct format");
        }
    }

    private T Read<T>(int typeSize)
        where T : unmanaged
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - Position >= typeSize, "Reached end of file!");

        var result = Unsafe.ReadUnaligned<T>(ref _byteBuffer[_position]);
        _position += typeSize;

        return result;
    }

    public byte Read8BitUnsignedInteger() => Read<byte>(sizeof(byte));
    public ushort Read16BitUnsignedInteger() => Read<ushort>(sizeof(ushort));
    public uint Read32BitUnsignedInteger() => Read<uint>(sizeof(uint));
    public int Read32BitSignedInteger() => Read<int>(sizeof(int));
    public ulong Read64BitUnsignedInteger() => Read<ulong>(sizeof(ulong));

    public ReadOnlySpan<byte> ReadBytes(int bufferSize)
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - Position >= bufferSize, "Reached end of file!");

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