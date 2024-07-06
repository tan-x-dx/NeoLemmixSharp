using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class StringDataComponentReader : ILevelDataReader
{
    private const int StackByteBufferSize = 256;

    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.StringDataSectionIdentifier;

    public StringDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;

        _stringIdLookup.Add(string.Empty);
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

            if (byteBuffer.Length < stringLength)
            {
                var heapBuffer = new byte[stringLength];
                byteBuffer = new Span<byte>(heapBuffer);
            }

            var stringBuffer = byteBuffer[..stringLength];
            reader.ReadBytes(stringBuffer);

            var actualString = utf8Encoding.GetString(stringBuffer);

            _stringIdLookup.Add(actualString);
        }
    }
}