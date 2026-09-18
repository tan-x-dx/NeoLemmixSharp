namespace NeoLemmixSharp.IO;

public static class DefaultFileExtensions
{
    public const string PngFileExtension = ".png";

    public const string LevelFileExtension = ".ullv";
    internal static ReadOnlySpan<byte> LevelFileFormatMagicBytes => "LemLevelUL"u8;

    public const string ReplayFileExtension = ".ullr";
    internal static ReadOnlySpan<byte> ReplayFileFormatMagicBytes => "LemReplayUL"u8;

    public const string LevelPackFileExtension = ".ullp";
    internal static ReadOnlySpan<byte> LevelPackFileFormatMagicBytes => "LemLevelPackUL"u8;

    public const string StyleFileExtension = ".ulst";
    internal static ReadOnlySpan<byte> StyleFileFormatMagicBytes => "LemStyleUL"u8;
}
