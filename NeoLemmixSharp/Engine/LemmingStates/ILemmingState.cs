namespace NeoLemmixSharp.Engine.LemmingStates;

public interface ILemmingState
{
    protected static LevelScreen Level => LevelScreen.CurrentLevel!;
    protected static LevelTerrain Terrain => LevelScreen.CurrentLevel!.Terrain;

    int LemmingStateId { get; }

    void UpdateLemming(Lemming lemming);
}