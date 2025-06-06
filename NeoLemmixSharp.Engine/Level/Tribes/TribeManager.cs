using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Tribes;

public sealed class TribeManager :
    IPerfectHasher<Tribe>,
    IBitBufferCreator<BitBuffer32>,
    IItemManager<Tribe>,
    IDisposable
{
    private readonly Tribe[] _tribes;
    public ReadOnlySpan<Tribe> AllItems => new(_tribes);

    public TribeManager(Tribe[] tribes)
    {
        _tribes = tribes;
        this.AssertUniqueIds(new ReadOnlySpan<Tribe>(_tribes));
        Array.Sort(_tribes, this);
    }

    public Tribe? GetTribeForId(int? id)
    {
        if (!id.HasValue)
            return null;

        var actualId = id.Value;
        if ((uint)actualId < (uint)_tribes.Length)
            return _tribes[actualId];

        return null;
    }

    public int NumberOfItems => _tribes.Length;

    int IPerfectHasher<Tribe>.Hash(Tribe item) => item.Id;
    Tribe IPerfectHasher<Tribe>.UnHash(int index) => _tribes[index];
    void IBitBufferCreator<BitBuffer32>.CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TribeSet CreateBitArraySet() => new(this);

    public void Dispose()
    {
        new Span<Tribe>(_tribes).Clear();
    }
}
