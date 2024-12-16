using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class DataReaderList : IDisposable
{
    private readonly NeoLemmixDataReader[] _dataReaders;
    private readonly FileStream _fileStream;
    private readonly StreamReader _streamReader;

    private NeoLemmixDataReader? _currentDataReader;

    public DataReaderList(
        string filePath,
        NeoLemmixDataReader[] dataReaders)
    {
        _dataReaders = dataReaders;

        _fileStream = new FileStream(filePath, FileMode.Open);
        _streamReader = new StreamReader(_fileStream);
    }

    public void ReadFile()
    {
        while (_streamReader.ReadLine() is { } line)
        {
            if (NxlvReadingHelpers.LineIsBlankOrComment(line))
                continue;

            bool reprocessLine;
            do
            {
                reprocessLine = ProcessLine(line);
            } while (reprocessLine);
        }
    }

    private bool ProcessLine(ReadOnlySpan<char> line)
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

    private void GetDataReaderForLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out _, out _);

        _currentDataReader = TryGetWithSpan(firstToken);
        if (_currentDataReader == null)
            throw new InvalidOperationException($"Could not find reader for line! [{firstToken}] line: \"{line}\"");

        _currentDataReader.BeginReading(line);
    }

    private NeoLemmixDataReader? TryGetWithSpan(ReadOnlySpan<char> token)
    {
        foreach (var reader in _dataReaders)
        {
            if (reader.ShouldProcessSection(token))
                return reader;
        }

        return null;
    }

    public void Dispose()
    {
        _streamReader.Dispose();
        _fileStream.Dispose();
    }
}