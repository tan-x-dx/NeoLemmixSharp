using System.Collections.Generic;

namespace NeoLemmixSharp.LevelBuilding.Data.SpriteSet;

public sealed class LemmingStateRecolouring
{
    public string StateIdentifier { get; }

    public List<(uint OriginalColour, uint ReplacementColour)> Recolourings { get; } = new();

    public LemmingStateRecolouring(string stateIdentifier)
    {
        StateIdentifier = stateIdentifier;
    }
}