namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class BinaryReaderWrapper : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly BinaryReader _reader;


    public long FileSizeInBytes => _fileStream.Length;
    public long BytesRead => _fileStream.Position;
    public bool MoreToRead => _fileStream.Position < _fileStream.Length;

    public BinaryReaderWrapper(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.Open);
        _reader = new BinaryReader(_fileStream);
    }

    public byte Read8BitUnsignedInteger()
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= 1, "Reached end of file!");

        return _reader.ReadByte();
    }

    public ushort Read16BitUnsignedInteger()
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= 2, "Reached end of file!");

        return _reader.ReadUInt16();
    }

    public uint Read32BitUnsignedInteger()
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= 4, "Reached end of file!");

        return _reader.ReadUInt32();
    }

    public int Read32BitSignedInteger()
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= 4, "Reached end of file!");

        return _reader.ReadInt32();
    }

    public ulong Read64BitUnsignedInteger()
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= 8, "Reached end of file!");

        return _reader.ReadUInt64();
    }

    public void ReadBytes(Span<byte> buffer)
    {
        LevelReadWriteHelpers.ReaderAssert(FileSizeInBytes - BytesRead >= buffer.Length, "Reached end of file!");

        var bytesRead = _reader.Read(buffer);

        LevelReadWriteHelpers.ReaderAssert(bytesRead == buffer.Length, "Reached end of file!");
    }

    public void Dispose()
    {
        _reader.Dispose();
        _fileStream.Dispose();
    }
}