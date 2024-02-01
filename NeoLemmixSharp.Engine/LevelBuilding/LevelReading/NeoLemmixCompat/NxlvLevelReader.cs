using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly DataReaderList _dataReaders = new();

    public NxlvLevelReader()
    {
        var terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();

        _dataReaders.Add(new LevelDataReader());
        _dataReaders.Add(new SkillSetReader());
        _dataReaders.Add(new TerrainGroupReader(terrainArchetypes));
        _dataReaders.Add(new TerrainReader(terrainArchetypes));
        _dataReaders.Add(new GadgetReader());
        _dataReaders.Add(new LemmingReader());
        _dataReaders.Add(new SpriteSetRecoloringReader());
        _dataReaders.Add(new StateRecoloringReader());
        _dataReaders.Add(new ShadesReader());

        _dataReaders.Add(new DudDataReader("$ANIMATIONS"));
    }

    public LevelData ReadLevel(string levelFilePath)
    {
        _dataReaders.ReadFile(levelFilePath);

        var levelData = new LevelData();

        //   ReadStyle();
        //  ReadSpriteData();
        //   SetUpGadgets();

        _dataReaders.ApplyToLevelData(levelData);

        return levelData;
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
                  _levelData.ThemeData.Mask = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                  break;

              case "MINIMAP":
                  _levelData.ThemeData.Minimap = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                  break;

              case "BACKGROUND":
                  _levelData.ThemeData.Background = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                  break;

              case "ONE_WAYS":
                  _levelData.ThemeData.OneWays = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                  break;

              case "PICKUP_BORDER":
                  _levelData.ThemeData.PickupBorder = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                  break;

              case "PICKUP_INSIDE":
                  _levelData.ThemeData.PickupInside = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
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
        _dataReaders.Dispose();
    }
}
