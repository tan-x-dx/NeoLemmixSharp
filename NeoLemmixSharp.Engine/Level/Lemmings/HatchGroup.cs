using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchGroup : IIdEquatable<HatchGroup>
{
    private readonly HatchGadget[] _hatches;

    private readonly int _minSpawnInterval;
    private readonly int _maxSpawnInterval;

    private int _hatchIndex;

    private int _spawnInterval;
    private int _spawnIntervalDelta;

    private int _nextLemmingCountDown = Global.InitialLemmingCountDown;
    private int _lemmingsToRelease;

    public int Id { get; }

    public HatchGroup(
        HatchGadget[] hatches,
        int minSpawnInterval,
        int maxSpawnInterval,
        int initialSpawnInterval)
    {
        _hatches = hatches;
        _hatchIndex = _hatches.Length - 1;
        _minSpawnInterval = minSpawnInterval;
        _maxSpawnInterval = maxSpawnInterval;
        _spawnInterval = initialSpawnInterval;
        _lemmingsToRelease = hatches.Sum(h => h.HatchSpawnData.LemmingsToRelease);
    }

    public void SetSpawnIntervalDelta(int spawnIntervalDelta)
    {
        _spawnIntervalDelta = spawnIntervalDelta;
    }

    public HatchGadget? Tick()
    {
        _spawnInterval = Math.Clamp(_spawnInterval + _spawnIntervalDelta, _minSpawnInterval, _maxSpawnInterval);

        _nextLemmingCountDown--;

        if (_nextLemmingCountDown != 0)
            return null;

        _nextLemmingCountDown = _spawnInterval;
        return GetNextLogicalHatchGadget();
    }

    private HatchGadget? GetNextLogicalHatchGadget()
    {
        if (_lemmingsToRelease == 0)
            return null;

        do
        {
            _hatchIndex++;
            if (_hatchIndex == _hatches.Length)
            {
                _hatchIndex = 0;
            }
            var hatchGadget = _hatches[_hatchIndex];

            if (hatchGadget.CanReleaseLemmings())
                return hatchGadget;
        } while (true);
    }

    public void OnSpawnLemming()
    {
        _lemmingsToRelease--;
    }

    public bool Equals(HatchGroup? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is HatchGroup other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(HatchGroup left, HatchGroup right) => left.Id == right.Id;
    public static bool operator !=(HatchGroup left, HatchGroup right) => left.Id != right.Id;
}