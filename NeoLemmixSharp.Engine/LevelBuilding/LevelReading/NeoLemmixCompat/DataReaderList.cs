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
        if (_currentDataReader != null)
        {
            var result = _currentDataReader.ReadNextLine(line);

            if (_currentDataReader.FinishedReading)
            {
                _currentDataReader = null;
            }
            return result;
        }

        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        if (!TryGetWithSpan(firstToken, out var dataReader))
            throw new InvalidOperationException($"Could not find reader for line! [{firstToken}] line: \"{line}\"");

        _currentDataReader = dataReader;
        _currentDataReader.BeginReading(line);

        return false;
    }

    private bool TryGetWithSpan(ReadOnlySpan<char> token, out INeoLemmixDataReader dataReader)
    {
        var dataReaderSpan = CollectionsMarshal.AsSpan(_dataReaders);

        foreach (var item in dataReaderSpan)
        {
            var itemSpan = item.IdentifierToken.AsSpan();

            if (itemSpan.SequenceEqual(token))
            {
                dataReader = item;
                return true;
            }
        }

        dataReader = null!;
        return false;
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