using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly INeoLemmixDataReader[] _dataReaders;
    private readonly List<NeoLemmixGadgetData> _neoLemmixGadgetData = new();

    private INeoLemmixDataReader? _currentDataReader;

    public NxlvLevelReader()
    {
        _dataReaders =
        [
            new LevelDataReader(),
            new SkillSetReader(),
            new TerrainGroupReader(),
            new GadgetReader(_neoLemmixGadgetData),
            new TerrainReader(),
            new LemmingReader(),
            new SpriteSetRecoloringReader(),
            new StateRecoloringReader(),
            new ShadesReader(),

            new DudDataReader("$ANIMATIONS")
        ];
    }

    public LevelData ReadLevel(string levelFilePath)
    {
        using (var stream = new FileStream(levelFilePath, FileMode.Open))
        {
            using var streamReader = new StreamReader(stream);

            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (LineIsBlankOrComment(line))
                    continue;

                bool reprocessLine;
                do
                {
                    reprocessLine = ProcessLine(line);
                } while (reprocessLine);
            }
        }

        var levelData = new LevelData();

        //   ReadStyle();
        //  ReadSpriteData();
        //   SetUpGadgets();

        foreach (var dataReader in _dataReaders)
        {
            dataReader.ApplyToLevelData(levelData);
        }

        return levelData;
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
        foreach (var item in _dataReaders)
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

    /*  private void ReadStyle()
      {
          var theme = _levelData.LevelTheme;
          var themeFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", theme, "theme.nxtm");

          using (var stream = new FileStream(themeFilePath, FileMode.Open))
          {
              using var streamReader = new StreamReader(stream);

              string? line;
              while ((line = streamReader.ReadLine()) != null)
              {
                  if (LineIsBlankOrComment(line))
                      continue;

                  ProcessThemeLine(line);
              }
          }

          _levelData.ThemeData.LemmingSpritesFilePath = Path.Combine(
              RootDirectoryManager.RootDirectory,
              "styles",
              _levelData.ThemeData.BaseStyle,
              "lemmings");
      }*/

    /*  private void ProcessThemeLine(string line)
      {
          var firstToken = ReadingHelpers.GetToken(line, 0, out _);
          var secondToken = ReadingHelpers.GetToken(line, 1, out _);

          switch (firstToken)
          {
              case "LEMMINGS":
                  _levelData.ThemeData.BaseStyle = secondToken.ToString();
                  break;

              case "$COLORS":
                  break;

              case "MASK":
                  _levelData.ThemeData.Mask = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "MINIMAP":
                  _levelData.ThemeData.Minimap = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "BACKGROUND":
                  _levelData.ThemeData.Background = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "ONE_WAYS":
                  _levelData.ThemeData.OneWays = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "PICKUP_BORDER":
                  _levelData.ThemeData.PickupBorder = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "PICKUP_INSIDE":
                  _levelData.ThemeData.PickupInside = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                  break;

              case "END":
                  break;
          }
      }*/

    /* private void ReadSpriteData()
     {
         var schemeFilePath = Path.Combine(
             _levelData.ThemeData.LemmingSpritesFilePath,
             "scheme.nxmi");

         using var stream = new FileStream(schemeFilePath, FileMode.Open);
         using var streamReader = new StreamReader(stream);

         string? line;
         while ((line = streamReader.ReadLine()) != null)
         {
             if (LineIsBlankOrComment(line))
                 continue;
             ProcessLine(line);
         }
     }*/

    public void Dispose()
    {
        foreach (var dataReader in _dataReaders)
        {
            dataReader.Dispose();
        }
    }

    private static bool LineIsBlankOrComment(string line)
    {
        return string.IsNullOrWhiteSpace(line) || line[0] == '#';
    }
}
