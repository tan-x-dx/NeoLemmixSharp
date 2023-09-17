namespace NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

public sealed class LemmingStateRecoloring
{
    public string StateIdentifier { get; }

    public List<(uint OriginalColor, uint ReplacementColor)> Recolorings { get; } = new();

    public LemmingStateRecoloring(string stateIdentifier)
    {
        StateIdentifier = stateIdentifier;
    }
}