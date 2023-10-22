using NeoLemmixSharp.Engine.Level.Gadgets.Functional;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchGroup
{
    private readonly HatchGadget[] _hatches;

    private readonly int _minSpawnInterval;
    private readonly int _maxSpawnInterval;

    private int _spawnInterval;
    private int _spawnIntervalDelta;

    private int _nextLemmingCountDown = Global.InitialLemmingCountDown;

    public HatchGroup(
        HatchGadget[] hatches,
        int minSpawnInterval,
        int maxSpawnInterval,
        int initialSpawnInterval)
    {
        _hatches = hatches;
        _minSpawnInterval = minSpawnInterval;
        _maxSpawnInterval = maxSpawnInterval;
        _spawnInterval = initialSpawnInterval;
    }

    public void SetSpawnIntervalDelta(int spawnIntervalDelta)
    {
        _spawnIntervalDelta = spawnIntervalDelta;
    }

    public void Tick()
    {
        _spawnInterval = Math.Clamp(_spawnInterval + _spawnIntervalDelta, _minSpawnInterval, _maxSpawnInterval);
        
        _nextLemmingCountDown--;

        if (_nextLemmingCountDown == 0)
        {
            _nextLemmingCountDown = _spawnInterval;
        }
    }
}