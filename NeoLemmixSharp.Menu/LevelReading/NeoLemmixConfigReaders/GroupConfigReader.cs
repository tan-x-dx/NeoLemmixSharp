using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class GroupConfigReader : NeoLemmixDataReader
{
    public GroupConfigReader(string identifierToken) : base(identifierToken)
    {
    }

    public override void BeginReading(ReadOnlySpan<char> line)
    {
        throw new NotImplementedException();
    }
}
