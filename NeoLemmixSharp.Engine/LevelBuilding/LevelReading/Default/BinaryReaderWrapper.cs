using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class BinaryReaderWrapper
{
    private const long MaxAllowedFileSizeInBytes = 1024 * 1024 * 64;

    private readonly byte[] _byteBuffer;
    private int _position;

    public int FileSizeInBytes => _byteBuffer.Length;
    public int BytesRead => _position;
    public bool MoreToRead => BytesRead < FileSizeInBytes;

    public BinaryReaderWrapper(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        var fileSizeInBytes = fileStream.Length;

        if (fileSizeInBytes > MaxAllowedFileSizeInBytes)
            throw new InvalidOperationException("File too large! Max file size is 64Mb");

        _byteBuffer = new byte[fileSizeInBytes];

        fileStream.ReadExactly(new Span<byte>(_byteBuffer));
    }

    private unsafe T Read<T>()
        where T : unmanaged
    {
        var typeSize = sizeof(T);
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= typeSize, "Reached end of file!");

        var span = MemoryMarshal.Cast<byte, T>(new ReadOnlySpan<byte>(_byteBuffer, _position, typeSize));
        _position += typeSize;

        return span[0];
    }

    public byte Read8BitUnsignedInteger() => Read<byte>();
    public ushort Read16BitUnsignedInteger() => Read<ushort>();
    public uint Read32BitUnsignedInteger() => Read<uint>();
    public int Read32BitSignedInteger() => Read<int>();
    public ulong Read64BitUnsignedInteger() => Read<ulong>();

    public ReadOnlySpan<byte> ReadBytes(int bufferSize)
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= bufferSize, "Reached end of file!");

        var sourceSpan = new ReadOnlySpan<byte>(_byteBuffer, _position, bufferSize);
        _position += bufferSize;

        return sourceSpan;
    }
}