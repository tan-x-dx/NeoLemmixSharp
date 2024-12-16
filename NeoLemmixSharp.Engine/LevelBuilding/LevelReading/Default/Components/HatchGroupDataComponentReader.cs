using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class HatchGroupDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.HatchGroupDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        while (numberOfItemsInSection-- > 0)
        {
            var byteBuffer = reader.ReadBytes(4);

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