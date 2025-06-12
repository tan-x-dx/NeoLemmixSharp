using System.Text;

namespace NeoLemmixSharp.IO.Reading;

internal readonly struct FileStringReader<TRawFileDataReader>(MutableFileReaderStringIdLookup stringIdLookup)
    where TRawFileDataReader : class, IRawFileDataReader
{
    private readonly MutableFileReaderStringIdLookup _stringIdLookup = stringIdLookup;

    internal void ReadSection(TRawFileDataReader rawFileData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(_stringIdLookup.Count == 0, "Expected string lookup to be empty!");
        _stringIdLookup.SetCapacity(numberOfItemsInSection + 1);
        _stringIdLookup.Add(string.Empty);

        while (numberOfItemsInSection-- > 0)
        {
            var actualString = ReadString(rawFileData);

            _stringIdLookup.Add(actualString);
        }
    }

    private string ReadString(TRawFileDataReader rawFileData)
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
