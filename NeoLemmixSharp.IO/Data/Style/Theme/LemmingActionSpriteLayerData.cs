using NeoLemmixSharp.Common.Enums;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct LemmingActionSpriteLayerData(int layer, TribeSpriteLayerColorType colorType) : IEquatable<LemmingActionSpriteLayerData>
{
    public readonly int Layer = layer;
    public readonly TribeSpriteLayerColorType ColorType = colorType;

    public bool Equals(LemmingActionSpriteLayerData other) => Layer == other.Layer && ColorType == other.ColorType;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is LemmingActionSpriteLayerData other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Layer, ColorType);
    public static bool operator ==(LemmingActionSpriteLayerData left, LemmingActionSpriteLayerData right) => left.Equals(right);
    public static bool operator !=(LemmingActionSpriteLayerData left, LemmingActionSpriteLayerData right) => !left.Equals(right);
}
