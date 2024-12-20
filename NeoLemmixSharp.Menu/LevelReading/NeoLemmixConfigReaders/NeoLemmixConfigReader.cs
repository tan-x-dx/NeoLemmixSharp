using NeoLemmixSharp.Menu.LevelPack;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public static class NeoLemmixConfigReader
{
    public static LevelPackData TryReadLevelPackData(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);
        var subFolders = Directory.GetDirectories(folderPath);


        return null!;
    }

    private static bool TryFindNeoLemmixConfigFile(ReadOnlySpan<string> files, ReadOnlySpan<char> fileNameToLocate, [MaybeNullWhen(false)] out string foundFilePath)
    {
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file.AsSpan());

            if (fileNameToLocate.SequenceEqual(file))
            {
                foundFilePath = file;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }
}
