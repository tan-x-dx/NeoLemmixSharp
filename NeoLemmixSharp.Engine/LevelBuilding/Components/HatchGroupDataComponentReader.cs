using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class HatchGroupDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.HatchGroupDataSectionIdentifier;

    [SkipLocalsInit]
    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        Span<byte> byteBuffer = stackalloc byte[4];
        while (numberOfItemsInSection-- > 0)
        {
            reader.ReadBytes(byteBuffer);

            levelData.AllHatchGroupData.Add(new HatchGroupData
            {
                HatchGroupId = byteBuffer[0],
                MinSpawnInterval = byteBuffer[1],
                MaxSpawnInterval = byteBuffer[2],
                InitialSpawnInterval = byteBuffer[3]
            });
        }
    }
}