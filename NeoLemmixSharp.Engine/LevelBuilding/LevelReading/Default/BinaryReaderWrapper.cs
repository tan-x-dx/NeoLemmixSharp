using NeoLemmixSharp.Engine.LevelBuilding.Components;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class BinaryReaderWrapper : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly BinaryReader _reader;
    private readonly long _fileSizeInBytes;

    private long _bytesRead;

    public long BytesRead => _bytesRead;
    public bool MoreToRead => _bytesRead < _fileSizeInBytes;

    public BinaryReaderWrapper(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.Open);
        _reader = new BinaryReader(_fileStream);

        _fileSizeInBytes = _fileStream.Length;
    }

    public byte Read8BitUnsignedInteger()
    {
        Helpers.ReaderAssert(_fileSizeInBytes - _bytesRead >= 1, "Reached end of file!");

        var result = _reader.ReadByte();
        _bytesRead++;
        return result;
    }

    public ushort Read16BitUnsignedInteger()
    {
        Helpers.ReaderAssert(_fileSizeInBytes - _bytesRead >= 2, "Reached end of file!");

        var result = _reader.ReadUInt16();
        _bytesRead += 2;
        return result;
    }

    public ulong Read64BitUnsignedInteger()
    {
        Helpers.ReaderAssert(_fileSizeInBytes - _bytesRead >= 8, "Reached end of file!");

        var result = _reader.ReadUInt64();
        _bytesRead += 8;
        return result;
    }

    public void ReadBytes(Span<byte> buffer)
    {
        Helpers.ReaderAssert(_fileSizeInBytes - _bytesRead >= buffer.Length, "Reached end of file!");

        var bytesRead = _reader.Read(buffer);

        Helpers.ReaderAssert(bytesRead == buffer.Length, "Reached end of file!");
        _bytesRead += bytesRead;
    }

    public void Dispose()
    {
        _reader.Dispose();
        _fileStream.Dispose();
    }
}