using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchGroup : IIdEquatable<HatchGroup>
{
    private HatchGadget[] _hatches;

    private int _hatchIndex;

    private int _nextLemmingCountDown = EngineConstants.InitialLemmingHatchReleaseCountDown;
    private int _lemmingsToRelease;

    public int Id { get; }

    public int MinSpawnInterval { get; }
    public int MaxSpawnInterval { get; }
    public int CurrentSpawnInterval { get; private set; }

    public int MinReleaseRate => ConvertToReleaseRate(MinSpawnInterval);
    public int MaxReleaseRate => ConvertToReleaseRate(MaxSpawnInterval);
    public int CurrentReleaseRate => ConvertToReleaseRate(CurrentSpawnInterval);

    public HatchGroup(
        int id,
        int minSpawnInterval,
        int maxSpawnInterval,
        int initialSpawnInterval)
    {
        Id = id;

        MinSpawnInterval = Math.Clamp(minSpawnInterval, EngineConstants.MinAllowedSpawnInterval, EngineConstants.MaxAllowedSpawnInterval);
        MaxSpawnInterval = Math.Clamp(maxSpawnInterval, minSpawnInterval, EngineConstants.MaxAllowedSpawnInterval);
        CurrentSpawnInterval = Math.Clamp(initialSpawnInterval, MinSpawnInterval, MaxSpawnInterval);
    }

    public void SetHatches(HatchGadget[] hatches)
    {
        _hatches = hatches;
        _hatchIndex = _hatches.Length - 1;

        var lemmingsToRelease = 0;
        foreach (var hatchGadget in hatches)
        {
            lemmingsToRelease += hatchGadget.HatchSpawnData.LemmingsToRelease;
        }

        _lemmingsToRelease = lemmingsToRelease;
    }

    public bool ChangeSpawnInterval(int spawnIntervalDelta)
    {
        var previousSpawnInterval = CurrentSpawnInterval;
        CurrentSpawnInterval = Math.Clamp(CurrentSpawnInterval + spawnIntervalDelta, MinSpawnInterval, MaxSpawnInterval);

        return previousSpawnInterval != CurrentSpawnInterval;
    }

    public HatchGadget? Tick()
    {
        if (_lemmingsToRelease == 0)
            return null;

        _nextLemmingCountDown--;

        if (_nextLemmingCountDown != 0)
            return null;

        _nextLemmingCountDown = CurrentSpawnInterval;
        return GetNextLogicalHatchGadget();
    }

    private HatchGadget? GetNextLogicalHatchGadget()
    {
        var c = _hatches.Length;

        do
        {
            _hatchIndex++;
            c--;
            if (_hatchIndex == _hatches.Length)
            {
                _hatchIndex = 0;
            }
            var hatchGadget = _hatches[_hatchIndex];

            if (hatchGadget.CanReleaseLemmings())
                return hatchGadget;
        } while (c > 0);

        return null;
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
        return 1 + EngineConstants.MaxAllowedSpawnInterval - spawnInterval;
    }

    public bool Equals(HatchGroup? other) => Id == (other?.Id ?? -1);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is HatchGroup other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(HatchGroup left, HatchGroup right) => left.Id == right.Id;
    public static bool operator !=(HatchGroup left, HatchGroup right) => left.Id != right.Id;
}