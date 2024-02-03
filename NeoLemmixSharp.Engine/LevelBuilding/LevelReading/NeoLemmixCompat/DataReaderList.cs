using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class DataReaderList : IDisposable
{
    private readonly List<INeoLemmixDataReader> _dataReaders = new();

    private INeoLemmixDataReader? _currentDataReader;

    public void Add(INeoLemmixDataReader dataReader)
    {
        _dataReaders.Add(dataReader);
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
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

        _currentDataReader = TryGetWithSpan(firstToken);
        if (_currentDataReader == null)
            throw new InvalidOperationException($"Could not find reader for line! [{firstToken}] line: \"{line}\"");

        _currentDataReader.BeginReading(line);
    }

    private INeoLemmixDataReader? TryGetWithSpan(ReadOnlySpan<char> token)
    {
        var dataReaderSpan = CollectionsMarshal.AsSpan(_dataReaders);

        foreach (var item in dataReaderSpan)
        {
            var itemSpan = item.IdentifierToken.AsSpan();

            if (itemSpan.SequenceEqual(token))
                return item;
        }

        return null;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        var span = CollectionsMarshal.AsSpan(_dataReaders);
        foreach (var dataReader in span)
        {
            dataReader.ApplyToLevelData(levelData);
        }
    }

    public void Dispose()
    {
        var span = CollectionsMarshal.AsSpan(_dataReaders);
        DisposableHelperMethods.DisposeOfAll((ReadOnlySpan<INeoLemmixDataReader>)span);
        _dataReaders.Clear();
    }
}