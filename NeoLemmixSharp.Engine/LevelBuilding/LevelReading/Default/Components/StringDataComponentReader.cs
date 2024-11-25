using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class StringDataComponentReader : ILevelDataReader
{
    private const int StackByteBufferSize = 256;

    private readonly Dictionary<ushort, string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.StringDataSectionIdentifier;

    public StringDataComponentReader(Dictionary<ushort, string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;

        _stringIdLookup.Add(0, string.Empty);
    }

    [SkipLocalsInit]
    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItems = reader.Read16BitUnsignedInteger();

        var utf8Encoding = Encoding.UTF8;

        Span<byte> byteBuffer = stackalloc byte[StackByteBufferSize];

        while (numberOfItems-- > 0)
        {
            var id = reader.Read16BitUnsignedInteger();
            LevelReadWriteHelpers.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            // The next 16bit int specifies how many bytes make up the next string
            // Read them into the buffer, and then parse those bytes into a string
            var stringLengthInBytes = reader.Read16BitUnsignedInteger();

            if (stringLengthInBytes > byteBuffer.Length)
            {
                var heapBuffer = new byte[stringLengthInBytes];
                byteBuffer = new Span<byte>(heapBuffer);
            }

            var stringBytes = byteBuffer[..stringLengthInBytes];
            reader.ReadBytes(stringBytes);

            var actualString = utf8Encoding.GetString(stringBytes);

            _stringIdLookup.Add(id, actualString);
        }
    }
}