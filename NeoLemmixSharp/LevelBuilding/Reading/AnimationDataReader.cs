using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using System;

namespace NeoLemmixSharp.LevelBuilding.Reading;

public sealed class AnimationDataReader : IDataReader
{
    private readonly ThemeData _themeData;

    private LemmingSpriteData? _currentLemmingSpriteData;

    public AnimationDataReader(ThemeData themeData)
    {
        _themeData = themeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$ANIMATIONS";
    public void BeginReading(string[] tokens)
    {
        _currentLemmingSpriteData = null;
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0][0] == '$')
        {
            if (tokens[0] == "$END")
            {
                if (_currentLemmingSpriteData == null)
                {
                    FinishedReading = true;
                    return;
                }

                _currentLemmingSpriteData = null;
                return;
            }

            if (_themeData.LemmingSpriteDataLookup.TryGetValue(tokens[0], out _currentLemmingSpriteData))
                return;

            _currentLemmingSpriteData = new LemmingSpriteData(tokens[0]);
            _themeData.LemmingSpriteDataLookup.Add(_currentLemmingSpriteData.AnimationIdentifier, _currentLemmingSpriteData);

            return;
        }

        switch (tokens[0])
        {
            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}