using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class ThemeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }

    public required Color Mask { get; init; }
    public required Color Minimap { get; init; }
    public required Color Background { get; init; }
    public required Color OneWayArrows { get; init; }
    public required Color PickupBorder { get; init; }
    public required Color PickupInside { get; init; }

    public LemmingActionSpriteDataBuffer LemmingActionSpriteData = new();
    public TribeColorDataBuffer TribeColorData = new();

    [InlineArray(EngineConstants.NumberOfLemmingActions)]
    public struct LemmingActionSpriteDataBuffer
    {
        private LemmingActionSpriteData _0;
    }

    [InlineArray(EngineConstants.MaxNumberOfTribes)]
    public struct TribeColorDataBuffer
    {
        private TribeColorData _0;
    }
}
