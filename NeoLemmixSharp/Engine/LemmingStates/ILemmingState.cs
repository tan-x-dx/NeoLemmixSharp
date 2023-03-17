namespace NeoLemmixSharp.Engine.LemmingStates;

public interface ILemmingState
{
    protected static LevelScreen Level => LevelScreen.CurrentLevel!;
    protected static LevelTerrain LevelTerrain => LevelScreen.CurrentLevel!.Terrain;

    int LemmingStateId { get; }

    void UpdateLemming(Lemming lemming);
}