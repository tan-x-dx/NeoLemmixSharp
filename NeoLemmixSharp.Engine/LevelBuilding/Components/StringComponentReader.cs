using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class StringComponentReader : ILevelDataReader
{
    private const int StringBufferSize = 1024;

    private readonly List<string> _stringIdLookup;
    private readonly byte[] _byteBuffer = new byte[StringBufferSize];

    public StringComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;

        _stringIdLookup.Add(string.Empty);
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x26, 0x44];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        var numberOfItems = reader.Read16BitUnsignedInteger();

        var utf8Encoding = Encoding.UTF8;

        while (numberOfItems > 0)
        {
            var id = reader.Read16BitUnsignedInteger();
            Helpers.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            var stringLength = reader.Read16BitUnsignedInteger();

            var stringBuffer = new Span<byte>(_byteBuffer, 0, stringLength);
            reader.ReadBytes(stringBuffer);

            var actualString = utf8Encoding.GetString(stringBuffer);

            _stringIdLookup.Add(actualString);

            numberOfItems--;
        }
    }
}