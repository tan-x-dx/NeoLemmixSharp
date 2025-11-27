using NeoLemmixSharp.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGroup : IEquatable<HatchGroup>
{
    private HatchGadget[] _hatches = [];

    private int _hatchIndex;

    private uint _nextLemmingCountDown = EngineConstants.InitialLemmingHatchReleaseCountDown;
    private int _lemmingsToRelease;

    public int Id { get; }

    public uint MinSpawnInterval { get; }
    public uint MaxSpawnInterval { get; }
    public uint CurrentSpawnInterval { get; private set; }

    public uint MinReleaseRate => ConvertToReleaseRate(MinSpawnInterval);
    public uint MaxReleaseRate => ConvertToReleaseRate(MaxSpawnInterval);
    public uint CurrentReleaseRate => ConvertToReleaseRate(CurrentSpawnInterval);
    public int LemmingsToRelease => _lemmingsToRelease;

    public HatchGroup(
        int id,
        uint minSpawnInterval,
        uint maxSpawnInterval,
        uint initialSpawnInterval)
    {
        Id = id;

        Debug.Assert(minSpawnInterval <= initialSpawnInterval);
        Debug.Assert(initialSpawnInterval <= maxSpawnInterval);

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

    public bool TryAddSpawnIntervalDelta(int spawnIntervalDelta)
    {
        int previousSpawnInterval = (int)CurrentSpawnInterval;
        int newSpawnInterval = Math.Clamp(previousSpawnInterval + spawnIntervalDelta, (int)MinSpawnInterval, (int)MaxSpawnInterval);
        CurrentSpawnInterval = (uint)newSpawnInterval;

        return previousSpawnInterval != newSpawnInterval;
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
        var hatchIndex = _hatchIndex;

        do
        {
            c--;
            hatchIndex++;
            if ((uint)hatchIndex >= (uint)_hatches.Length)
                hatchIndex = 0;

            var hatchGadget = _hatches[hatchIndex];

            if (hatchGadget.CanReleaseLemmings())
            {
                _hatchIndex = hatchIndex;
                return hatchGadget;
            }
        } while (c > 0);

        _hatchIndex = hatchIndex;
        return null;
    }

    public void OnSpawnLemming()
    {
        _lemmingsToRelease--;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ConvertToReleaseRate(uint spawnInterval)
    {
        // So that:
        // RR1 <=> SI102,
        // RR50 <=> SI53,
        // RR99 <=> SI4, etc
        return 1 + EngineConstants.MaxAllowedSpawnInterval - spawnInterval;
    }

    public bool Equals(HatchGroup? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is HatchGroup other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(HatchGroup? left, HatchGroup? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }
    public static bool operator !=(HatchGroup? left, HatchGroup? right) => !(left == right);
}
