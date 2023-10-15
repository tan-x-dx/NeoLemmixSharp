namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Reading;

public static class ReadingHelpers
{
    public static bool TrySplitIntoTokens(string line, out string[] tokens)
    {
        if (line.Length == 0 || line[0] == '#') // comment/blank line - ignore
        {
            tokens = Array.Empty<string>();
            return false;
        }

        tokens = line.Trim().Split(' ', StringSplitOptions.TrimEntries);
        if (tokens.Length == 1 && tokens[0].Length == 0) // blank line
        {
            tokens = Array.Empty<string>();
            return false;
        }

        return true;
    }

    public static string ReadFormattedString(string[] tokens)
    {
        return string.Join(' ', tokens);
    }

    public static ulong ReadUlong(string token)
    {
        if (token[0] == 'x')
        {
            token = $"0{token}";
        }

        return Convert.ToUInt64(token, 16);
    }

    public static int ReadInt(string token)
    {
        return int.Parse(token);
    }

    public static uint ReadUint(string token, bool parseAlpha)
    {
        if (token[0] == 'x')
        {
            token = $"0{token}";
        }

        var result = Convert.ToUInt32(token, 16);
        if (parseAlpha)
            return result;

        return result | 0xff000000U;
    }
}