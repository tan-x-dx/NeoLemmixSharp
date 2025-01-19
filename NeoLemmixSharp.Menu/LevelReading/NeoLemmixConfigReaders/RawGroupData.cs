namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public readonly struct RawGroupData
{
    public readonly string Name;
    public readonly string FolderPath;

    public RawGroupData(
        string name,
        string folderPath)
    {
        Name = name;
        FolderPath = folderPath;
    }
}
