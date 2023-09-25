using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchGroup : IIdEquatable<HatchGroup>
{
    private readonly HatchGadget[] _hatches;

    public int Id { get; }
    public int MinSpawnInterval { get; }
    public int MaxSpawnInterval { get; }

    public HatchGroup(
        int id,
        HatchGadget[] hatches)
    {
        Id = id;
        _hatches = hatches;
    }

    public bool Equals(HatchGroup? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is HatchGroup other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(HatchGroup left, HatchGroup right) => left.Id == right.Id;
    public static bool operator !=(HatchGroup left, HatchGroup right) => left.Id != right.Id;
}