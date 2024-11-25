using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class BinaryReaderWrapper
{
    private readonly byte[] _byteBuffer;
    private int _position;

    public long FileSizeInBytes => _byteBuffer.Length;
    public long BytesRead => _position;
    public bool MoreToRead => BytesRead < FileSizeInBytes;

    public BinaryReaderWrapper(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        _byteBuffer = new byte[fileStream.Length];

        fileStream.ReadExactly(new Span<byte>(_byteBuffer));
    }

    private T ReadWord<T>()
        where T : unmanaged
    {
        var typeSize = Unsafe.SizeOf<T>();
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= typeSize, "Reached end of file!");

        var span = MemoryMarshal.Cast<byte, T>(new ReadOnlySpan<byte>(_byteBuffer, _position, typeSize));
        _position += typeSize;

        return span[0];
    }

    public byte Read8BitUnsignedInteger() => ReadWord<byte>();

    public ushort Read16BitUnsignedInteger() => ReadWord<ushort>();

    public uint Read32BitUnsignedInteger() => ReadWord<uint>();

    public int Read32BitSignedInteger() => ReadWord<int>();

    public ulong Read64BitUnsignedInteger() => ReadWord<ulong>();

    public void ReadBytes(Span<byte> buffer)
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= buffer.Length, "Reached end of file!");

        var sourceSpan = new ReadOnlySpan<byte>(_byteBuffer, _position, buffer.Length);
        _position += buffer.Length;

        sourceSpan.CopyTo(buffer);
    }
}