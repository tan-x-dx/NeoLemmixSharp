using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class BackgroundData
{
    public Color Color { get; }
    public string BackgroundImageName { get; }

    public bool IsSolidColor => string.IsNullOrWhiteSpace(BackgroundImageName);

    internal BackgroundData(Color color)
    {
        Color = color;
        BackgroundImageName = string.Empty;
    }

    internal BackgroundData(string backgroundImageName)
    {
        Color = Color.Black;
        BackgroundImageName = backgroundImageName;
    }
}
