using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class StringDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.StringDataSectionIdentifier;

    public StringDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;

        _stringIdLookup.Add(string.Empty);
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItems = reader.Read16BitUnsignedInteger();

        var utf8Encoding = Encoding.UTF8;

        while (numberOfItems-- > 0)
        {
            int id = reader.Read16BitUnsignedInteger();
            LevelReadWriteHelpers.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            // The next 16bit int specifies how many bytes make up the next string
            int stringLengthInBytes = reader.Read16BitUnsignedInteger();
            var stringBytes = reader.ReadBytes(stringLengthInBytes);
            var actualString = utf8Encoding.GetString(stringBytes);

            _stringIdLookup.Add(actualString);
        }
    }
}