using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotRecorder : IDisposable
{
    private readonly RawArray _workBuffer;
    private int _snapshotBufferCapacity;
    private RawArray _snapshotBuffer;
    private int _numberOfSnapshots;

    private bool _isDisposed;

    private int TotalNumberOfBytesPerSnapshot => _workBuffer.Length;

    public SnapshotRecorder(RawArray workBuffer)
    {
        _workBuffer = workBuffer;

        _snapshotBufferCapacity = RewindConstants.SnapshotDataListSizeMultiplier;
        _snapshotBuffer = Helpers.AllocateBuffer<byte>(TotalNumberOfBytesPerSnapshot * RewindConstants.SnapshotDataListSizeMultiplier);
    }

    public unsafe void TakeSnapshot()
    {
        byte* snapshotDataPointer = GetNewSnapshotDataPointer();
        byte* workBufferPointer = (byte*)_workBuffer.Handle;

        var sourceSpan = Helpers.CreateReadOnlySpan<byte>(workBufferPointer, TotalNumberOfBytesPerSnapshot);
        var destinationSpan = Helpers.CreateSpan<byte>(snapshotDataPointer, TotalNumberOfBytesPerSnapshot);

        sourceSpan.CopyTo(destinationSpan);
    }

    private unsafe byte* GetNewSnapshotDataPointer()
    {
        if (_numberOfSnapshots == _snapshotBufferCapacity)
            DoubleSnapshotBufferCapacity();

        byte* pointer = (byte*)_snapshotBuffer.Handle + (_numberOfSnapshots * TotalNumberOfBytesPerSnapshot);
        _numberOfSnapshots++;

        return pointer;
    }

    private void DoubleSnapshotBufferCapacity()
    {
        RawArray.DoubleBufferSize(ref _snapshotBuffer);
        _snapshotBufferCapacity *= 2;
    }

    public unsafe void ApplySnapshot(int snapshotNumber)
    {
        byte* snapshotDataPointer = GetDataPointerForSnapshotNumber(snapshotNumber);
        byte* workBufferPointer = (byte*)_workBuffer.Handle;

        var sourceSpan = Helpers.CreateReadOnlySpan<byte>(snapshotDataPointer, TotalNumberOfBytesPerSnapshot);
        var destinationSpan = Helpers.CreateSpan<byte>(workBufferPointer, TotalNumberOfBytesPerSnapshot);

        sourceSpan.CopyTo(destinationSpan);
    }

    private unsafe byte* GetDataPointerForSnapshotNumber(int snapshotNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(snapshotNumber);

        _numberOfSnapshots = snapshotNumber + 1;

        byte* pointer = (byte*)_snapshotBuffer.Handle + (snapshotNumber * TotalNumberOfBytesPerSnapshot);
        return pointer;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            _snapshotBuffer.Dispose();
            _workBuffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
