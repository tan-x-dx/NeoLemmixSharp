using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class DataReaderList
{
    private readonly INeoLemmixDataReader[] _dataReaders;

    private INeoLemmixDataReader? _currentDataReader;

    public DataReaderList(INeoLemmixDataReader[] dataReaders)
    {
        _dataReaders = dataReaders;
    }

    public void ReadFile(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        using var streamReader = new StreamReader(stream);

        while (streamReader.ReadLine() is { } line)
        {
            if (ReadingHelpers.LineIsBlankOrComment(line))
                continue;

            bool reprocessLine;
            do
            {
                reprocessLine = ProcessLine(line);
            } while (reprocessLine);
        }
    }

    private bool ProcessLine(string line)
    {
        if (_currentDataReader == null)
        {
            GetDataReaderForLine(line);

            return false;
        }

        var result = _currentDataReader.ReadNextLine(line);

        if (_currentDataReader.FinishedReading)
        {
            _currentDataReader = null;
        }
        return result;
    }

    private void GetDataReaderForLine(string line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out _, out _);

        _currentDataReader = TryGetWithSpan(firstToken);
        if (_currentDataReader == null)
            throw new InvalidOperationException($"Could not find reader for line! [{firstToken}] line: \"{line}\"");

        _currentDataReader.BeginReading(line);
    }

    private INeoLemmixDataReader? TryGetWithSpan(ReadOnlySpan<char> token)
    {
        foreach (var item in _dataReaders)
        {
            if (item.MatchesToken(token))
                return item;
        }

        return null;
    }
}