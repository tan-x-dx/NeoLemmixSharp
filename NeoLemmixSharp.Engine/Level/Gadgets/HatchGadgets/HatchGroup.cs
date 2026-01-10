using NeoLemmixSharp.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    private ref int HatchIndex => ref _data.HatchIndex;
    private ref uint NextLemmingCountDown => ref _data.NextLemmingCountDown;
    private ref int LemmingsToReleaseRef => ref _data.LemmingsToRelease;
    public int LemmingsToRelease => _data.LemmingsToRelease;
    private ref uint CurrentSpawnIntervalRef => ref _data.CurrentSpawnInterval;
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
        CurrentSpawnIntervalRef = Math.Clamp(initialSpawnInterval, MinSpawnInterval, MaxSpawnInterval);
        NextLemmingCountDown = EngineConstants.InitialLemmingHatchReleaseCountDown;
    }

    public void SetHatches(HatchGadget[] hatches)
    {
        _hatches = hatches;
        HatchIndex = _hatches.Length - 1;

        var lemmingsToRelease = 0;
        foreach (var hatchGadget in hatches)
        {
            lemmingsToRelease += hatchGadget.HatchSpawnData.LemmingsToRelease;
        }

        LemmingsToReleaseRef = lemmingsToRelease;
    }

    public bool TryAddSpawnIntervalDelta(int spawnIntervalDelta)
    {
        int previousSpawnInterval = (int)CurrentSpawnIntervalRef;
        int newSpawnInterval = previousSpawnInterval + spawnIntervalDelta;
        if (newSpawnInterval > (int)MaxSpawnInterval)
            newSpawnInterval = (int)MaxSpawnInterval;
        else if (newSpawnInterval < (int)MinSpawnInterval)
            newSpawnInterval = (int)MinSpawnInterval;

        CurrentSpawnIntervalRef = (uint)newSpawnInterval;

        return previousSpawnInterval != newSpawnInterval;
    }

    public HatchGadget? Tick()
    {
        if (LemmingsToReleaseRef == 0)
            return null;

        NextLemmingCountDown--;

        if (NextLemmingCountDown != 0)
            return null;

        NextLemmingCountDown = CurrentSpawnIntervalRef;
        return GetNextLogicalHatchGadget();
    }

    private HatchGadget? GetNextLogicalHatchGadget()
    {
        var c = _hatches.Length;
        var hatchIndex = HatchIndex;

        do
        {
            c--;
            hatchIndex++;
            if ((uint)hatchIndex >= (uint)_hatches.Length)
                hatchIndex = 0;

            var hatchGadget = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_hatches), hatchIndex);

            if (hatchGadget.CanReleaseLemmings())
            {
                HatchIndex = hatchIndex;
                return hatchGadget;
            }
        } while (c > 0);

        HatchIndex = hatchIndex;
        return null;
    }

    public void OnSpawnLemming()
    {
        LemmingsToReleaseRef--;
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
