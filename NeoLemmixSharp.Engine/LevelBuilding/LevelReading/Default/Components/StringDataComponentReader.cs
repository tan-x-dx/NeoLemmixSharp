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

            var stringLength = reader.Read16BitUnsignedInteger();

            if (stringLength > byteBuffer.Length)
            {
                var heapBuffer = new byte[stringLength];
                byteBuffer = new Span<byte>(heapBuffer);
            }

            var stringBuffer = byteBuffer[..stringLength];
            reader.ReadBytes(stringBuffer);

            var actualString = utf8Encoding.GetString(stringBuffer);

            _stringIdLookup.Add(id, actualString);
        }
    }
}