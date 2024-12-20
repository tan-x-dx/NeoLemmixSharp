namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class PackInfoData
{
    public required string Title { get; init; }
    public required string Author { get; init; }

    public List<string> Messages { get; } = [];

    public static PackInfoData Default { get; }

    static PackInfoData()
    {
        Default = new PackInfoData()
        {
            Title = string.Empty,
            Author = string.Empty,
        };

        Default.Messages.AddRange(
            string.Empty,
            string.Empty,
            string.Empty,
            "Thanks to...",
            "DMA for the original game",
            "EricLang & ccexplore for Lemmix",
            "namida & Nepster for NeoLemmix",
            "Alex A.Denisov for Graphics32",
            "base2 technologies for ZLibEx",
            "Un4seen Development for Bass.dll",
            "The Lemmings Forums community at",
            "http://www.lemmingsforums.net",
            "Volker Oth and Mindless for",
            "sharing sourcecode and information",
            "What are you waiting for?",
            "Go play the game already!");
    }
}
