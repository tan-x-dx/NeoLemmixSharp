using NeoLemmixSharp.Common;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingColorManager : IDisposable
{
    private readonly RawArray _lemmingColorData;

    public LemmingColorManager(RawArray lemmingColorManagerDataBuffer)
    {
        _lemmingColorData = lemmingColorManagerDataBuffer;
    }

    public unsafe ref LemmingColorData GetLemmingColorData(int lemmingId)
    {
        LemmingColorData* p = (LemmingColorData*)_lemmingColorData.Handle;
        p += lemmingId;

        return ref Unsafe.AsRef<LemmingColorData>(p);
    }

    public void Dispose() => _lemmingColorData.Dispose();
}
