using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGroup : IEquatable<HatchGroup>
{
    private readonly HatchGroupData _data;

    private HatchGadget[] _hatches = [];
    public int Id { get; }

    public uint MinSpawnInterval { get; }
    public uint MaxSpawnInterval { get; }

    public uint MinReleaseRate => ConvertToReleaseRate(MinSpawnInterval);
    public uint MaxReleaseRate => ConvertToReleaseRate(MaxSpawnInterval);
    public uint CurrentReleaseRate => ConvertToReleaseRate(CurrentSpawnInterval);

    public int LemmingsToRelease => _data.LemmingsToRelease;
    public uint CurrentSpawnInterval => _data.CurrentSpawnInterval;

    public HatchGroup(
        nint dataHandle,
        int id,
        uint minSpawnInterval,
        uint maxSpawnInterval,
        uint initialSpawnInterval)
    {
        _data = new HatchGroupData(dataHandle);
        Id = id;

        Debug.Assert(minSpawnInterval <= initialSpawnInterval);
        Debug.Assert(initialSpawnInterval <= maxSpawnInterval);

        MinSpawnInterval = Math.Clamp(minSpawnInterval, EngineConstants.MinAllowedSpawnInterval, EngineConstants.MaxAllowedSpawnInterval);
        MaxSpawnInterval = Math.Clamp(maxSpawnInterval, minSpawnInterval, EngineConstants.MaxAllowedSpawnInterval);
        _data.CurrentSpawnInterval = Math.Clamp(initialSpawnInterval, MinSpawnInterval, MaxSpawnInterval);
        _data.NextLemmingCountDown = EngineConstants.InitialLemmingHatchReleaseCountDown;
    }

    public void SetHatches(HatchGadget[] hatches)
    {
        _hatches = hatches;
        _data.HatchIndex = _hatches.Length - 1;

        var lemmingsToRelease = 0;
        foreach (var hatchGadget in hatches)
        {
            lemmingsToRelease += hatchGadget.HatchSpawnData.LemmingsToRelease;
        }

        _data.LemmingsToRelease = lemmingsToRelease;
    }

    public bool TryAddSpawnIntervalDelta(int spawnIntervalDelta)
    {
        int previousSpawnInterval = (int)_data.CurrentSpawnInterval;
        int newSpawnInterval = previousSpawnInterval + spawnIntervalDelta;
        if (newSpawnInterval > (int)MaxSpawnInterval)
            newSpawnInterval = (int)MaxSpawnInterval;
        else if (newSpawnInterval < (int)MinSpawnInterval)
            newSpawnInterval = (int)MinSpawnInterval;

        _data.CurrentSpawnInterval = (uint)newSpawnInterval;

        return previousSpawnInterval != newSpawnInterval;
    }

    public HatchGadget? Tick()
    {
        if (_data.LemmingsToRelease == 0)
            return null;

        _data.NextLemmingCountDown--;

        if (_data.NextLemmingCountDown != 0)
            return null;

        _data.NextLemmingCountDown = _data.CurrentSpawnInterval;
        return GetNextLogicalHatchGadget();
    }

    private HatchGadget? GetNextLogicalHatchGadget()
    {
        var c = _hatches.Length - 1;
        if (c < 0)
            return null;

        var hatchIndex = _data.HatchIndex;

        do
        {
            hatchIndex++;
            if ((uint)hatchIndex >= (uint)_hatches.Length)
                hatchIndex = 0;

            var hatchGadget = _hatches.At(hatchIndex);

            if (hatchGadget.CanReleaseLemmings())
            {
                _data.HatchIndex = hatchIndex;
                return hatchGadget;
            }
            c--;
        } while (c >= 0);

        _data.HatchIndex = hatchIndex;
        return null;
    }

    public void OnSpawnLemming()
    {
        _data.LemmingsToRelease--;
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
