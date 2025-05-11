using NeoLemmixSharp.IO.Writing;
using System.Text;

namespace NeoLemmixSharp.IO.Reading;

public readonly struct FileStringReader<TPerfectHasher, TEnum>(List<string> stringIdLookup)
    where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
    where TEnum : unmanaged, Enum
{
    private readonly List<string> _stringIdLookup = stringIdLookup;

    public void ReadSection(RawFileDataReader<TPerfectHasher, TEnum> rawFileData)
    {
        int numberOfItems = rawFileData.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(_stringIdLookup.Count == 0, "Expected string list to be empty!");
        _stringIdLookup.Capacity = numberOfItems + 1;
        _stringIdLookup.Add(string.Empty);

        while (numberOfItems-- > 0)
        {
            var actualString = ReadString(rawFileData);

            _stringIdLookup.Add(actualString);
        }
    }

    private string ReadString(RawFileDataReader<TPerfectHasher, TEnum> rawFileData)
    {
        int id = rawFileData.Read16BitUnsignedInteger();
        FileReadingException.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

        // The next 16bit int specifies how many bytes make up the next string
        int stringLengthInBytes = rawFileData.Read16BitUnsignedInteger();
        var stringBytes = rawFileData.ReadBytes(stringLengthInBytes);
        var actualString = Encoding.UTF8.GetString(stringBytes);

        return actualString;
    }
}
