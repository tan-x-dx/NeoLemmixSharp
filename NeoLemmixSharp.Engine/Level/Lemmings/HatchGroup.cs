using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchGroup : IIdEquatable<HatchGroup>
{
    private readonly HatchGadget[] _hatches;

    public int Id { get; }

    public int MinSpawnInterval { get; }
    public int MaxSpawnInterval { get; }
    public int CurrentSpawnInterval { get; private set; }

    private int _hatchIndex;

    private int _nextLemmingCountDown = LevelConstants.InitialLemmingHatchReleaseCountDown;
    private int _lemmingsToRelease;

    public int MinReleaseRate => ConvertToReleaseRate(MinSpawnInterval);
    public int MaxReleaseRate => ConvertToReleaseRate(MaxSpawnInterval);
    public int CurrentReleaseRate => ConvertToReleaseRate(CurrentSpawnInterval);

    public HatchGroup(
        int id,
        HatchGadget[] hatches,
        int minSpawnInterval,
        int maxSpawnInterval,
        int initialSpawnInterval)
    {
        Id = id;

        _hatches = hatches;
        _hatchIndex = _hatches.Length - 1;
        MinSpawnInterval = Math.Clamp(minSpawnInterval, LevelConstants.MinAllowedSpawnInterval, LevelConstants.MaxAllowedSpawnInterval);
        MaxSpawnInterval = Math.Clamp(maxSpawnInterval, minSpawnInterval, LevelConstants.MaxAllowedSpawnInterval);
        CurrentSpawnInterval = Math.Clamp(initialSpawnInterval, MinSpawnInterval, MaxSpawnInterval);
        _lemmingsToRelease = hatches.Sum(h => h.HatchSpawnData.LemmingsToRelease);
    }

    public void ChangeSpawnInterval(int spawnIntervalDelta)
    {
        CurrentSpawnInterval = Math.Clamp(CurrentSpawnInterval + spawnIntervalDelta, MinSpawnInterval, MaxSpawnInterval);
    }

    public HatchGadget? Tick()
    {
        _nextLemmingCountDown--;

        if (_nextLemmingCountDown != 0)
            return null;

        _nextLemmingCountDown = CurrentSpawnInterval;
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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ConvertToReleaseRate(int spawnInterval)
    {
        // So that:
        // RR1 <=> SI102,
        // RR50 <=> SI53,
        // RR99 <=> SI4, etc
        return 1 + LevelConstants.MaxAllowedSpawnInterval - spawnInterval;
    }

    public bool Equals(HatchGroup? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is HatchGroup other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(HatchGroup left, HatchGroup right) => left.Id == right.Id;
    public static bool operator !=(HatchGroup left, HatchGroup right) => left.Id != right.Id;
}