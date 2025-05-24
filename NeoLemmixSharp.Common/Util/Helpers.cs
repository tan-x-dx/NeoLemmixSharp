using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class Helpers
{
    public const int Uint16NumberBufferLength = 5;
    public const int Uint32NumberBufferLength = 10;
    public const int Int32NumberBufferLength = Uint32NumberBufferLength + 1;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle CreateRectangle(Point pos, Size size) => new(pos.X, pos.Y, size.W, size.H);

    [Pure]
    public static int CountIfNotNull<T>(this T? item)
    where T : class
    {
        return item is not null ? 1 : 0;
    }

    [Pure]
    public static int CountIfNotNull<T>(this T? item)
    where T : struct
    {
        return item.HasValue ? 1 : 0;
    }

    public readonly ref struct FormatParameters(char openBracket, char separator, char closeBracket)
    {
        public readonly char OpenBracket = openBracket;
        public readonly char Separator = separator;
        public readonly char CloseBracket = closeBracket;
    }

    public static bool TryFormatSpan(
    ReadOnlySpan<int> source,
    Span<char> destination,
    out int charsWritten)
    {
        var formatParameters = new FormatParameters('(', ',', ')');
        return TryFormatSpan(source, destination, formatParameters, out charsWritten);
    }

    public static bool TryFormatSpan(
    ReadOnlySpan<int> source,
    Span<char> destination,
    FormatParameters formatParameters,
    out int charsWritten)
    {
        charsWritten = 0;

        if (source.Length == 0)
        {
            if (destination.Length < 2)
                return false;
            destination[charsWritten++] = formatParameters.OpenBracket;
            destination[charsWritten++] = formatParameters.CloseBracket;

            return true;
        }

        if (destination.Length < 3 + source.Length)
            return false;
        destination[charsWritten++] = formatParameters.OpenBracket;

        var l = source.Length - 1;
        bool couldWriteInt;
        int di;
        for (var j = 0; j < l; j++)
        {
            couldWriteInt = source[j].TryFormat(destination[charsWritten..], out di);
            charsWritten += di;
            if (!couldWriteInt)
                return false;

            if (charsWritten == destination.Length)
                return false;
            destination[charsWritten++] = formatParameters.Separator;
        }

        couldWriteInt = source[l].TryFormat(destination[charsWritten..], out di);
        charsWritten += di;
        if (!couldWriteInt)
            return false;

        if (charsWritten == destination.Length)
            return false;
        destination[charsWritten++] = formatParameters.CloseBracket;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum GetEnumValue<TEnum>(uint rawValue, uint numberOfEnumValues)
    where TEnum : unmanaged, Enum
    {
        var enumValue = Unsafe.As<uint, TEnum>(ref rawValue);
        if (rawValue < numberOfEnumValues)
            return enumValue;

        return ThrowUnknownEnumValueException<TEnum, TEnum>(enumValue);
    }

    [DoesNotReturn]
    public static TReturn ThrowUnknownEnumValueException<TEnum, TReturn>(TEnum enumValue)
    where TEnum : unmanaged, Enum
    {
        var typeName = typeof(TEnum).Name;
        throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, $"Unknown {typeName} enum value!");
    }

    #region Lemming Action Helpers

    private static readonly Dictionary<string, int> LemmingActionNameToIdLookup = GenerateLemmingActionNameToIdLookup();

    private static Dictionary<string, int> GenerateLemmingActionNameToIdLookup()
    {
        var result = new Dictionary<string, int>(EngineConstants.NumberOfLemmingActions, StringComparer.OrdinalIgnoreCase)
        {
            [EngineConstants.WalkerActionName] = EngineConstants.WalkerActionId,
            [EngineConstants.ClimberActionName] = EngineConstants.ClimberActionId,
            [EngineConstants.FloaterActionName] = EngineConstants.FloaterActionId,
            [EngineConstants.BlockerActionName] = EngineConstants.BlockerActionId,
            [EngineConstants.BuilderActionName] = EngineConstants.BuilderActionId,
            [EngineConstants.BasherActionName] = EngineConstants.BasherActionId,
            [EngineConstants.MinerActionName] = EngineConstants.MinerActionId,
            [EngineConstants.DiggerActionName] = EngineConstants.DiggerActionId,
            [EngineConstants.PlatformerActionName] = EngineConstants.PlatformerActionId,
            [EngineConstants.StackerActionName] = EngineConstants.StackerActionId,
            [EngineConstants.FencerActionName] = EngineConstants.FencerActionId,
            [EngineConstants.GliderActionName] = EngineConstants.GliderActionId,
            [EngineConstants.JumperActionName] = EngineConstants.JumperActionId,
            [EngineConstants.SwimmerActionName] = EngineConstants.SwimmerActionId,
            [EngineConstants.ShimmierActionName] = EngineConstants.ShimmierActionId,
            [EngineConstants.LasererActionName] = EngineConstants.LasererActionId,
            [EngineConstants.SliderActionName] = EngineConstants.SliderActionId,
            [EngineConstants.FallerActionName] = EngineConstants.FallerActionId,
            [EngineConstants.AscenderActionName] = EngineConstants.AscenderActionId,
            [EngineConstants.ShruggerActionName] = EngineConstants.ShruggerActionId,
            [EngineConstants.DrownerActionName] = EngineConstants.DrownerActionId,
            [EngineConstants.HoisterActionName] = EngineConstants.HoisterActionId,
            [EngineConstants.DehoisterActionName] = EngineConstants.DehoisterActionId,
            [EngineConstants.ReacherActionName] = EngineConstants.ReacherActionId,
            [EngineConstants.DisarmerActionName] = EngineConstants.DisarmerActionId,
            [EngineConstants.ExiterActionName] = EngineConstants.ExiterActionId,
            [EngineConstants.ExploderActionName] = EngineConstants.ExploderActionId,
            [EngineConstants.OhNoerActionName] = EngineConstants.OhNoerActionId,
            [EngineConstants.SplatterActionName] = EngineConstants.SplatterActionId,
            [EngineConstants.StonerActionName] = EngineConstants.StonerActionId,
            [EngineConstants.VaporiserActionName] = EngineConstants.VaporiserActionId,
            [EngineConstants.RotateClockwiseActionName] = EngineConstants.RotateClockwiseActionId,
            [EngineConstants.RotateCounterclockwiseActionName] = EngineConstants.RotateCounterclockwiseActionId,
            [EngineConstants.RotateHalfActionName] = EngineConstants.RotateHalfActionId
        };

        if (result.Count != EngineConstants.NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;
    }

    public static int GetLemmingActionIdFromName(string lemmingActionName)
    {
        return LemmingActionNameToIdLookup[lemmingActionName];
    }

    private static readonly LemmingActionLookupData[] LemmingActionIdToStringLookup = GenerateLemmingActionIdToStringLookup();

    private static LemmingActionLookupData[] GenerateLemmingActionIdToStringLookup()
    {
        var result = new LemmingActionLookupData[EngineConstants.NumberOfLemmingActions];
        var count = 0;

        SetData(EngineConstants.WalkerActionId, EngineConstants.WalkerActionName, EngineConstants.WalkerActionSpriteFileName, EngineConstants.WalkerAnimationFrames);
        SetData(EngineConstants.ClimberActionId, EngineConstants.ClimberActionName, EngineConstants.ClimberActionSpriteFileName, EngineConstants.ClimberAnimationFrames);
        SetData(EngineConstants.FloaterActionId, EngineConstants.FloaterActionName, EngineConstants.FloaterActionSpriteFileName, EngineConstants.FloaterAnimationFrames);
        SetData(EngineConstants.BlockerActionId, EngineConstants.BlockerActionName, EngineConstants.BlockerActionSpriteFileName, EngineConstants.BlockerAnimationFrames);
        SetData(EngineConstants.BuilderActionId, EngineConstants.BuilderActionName, EngineConstants.BuilderActionSpriteFileName, EngineConstants.BuilderAnimationFrames);
        SetData(EngineConstants.BasherActionId, EngineConstants.BasherActionName, EngineConstants.BasherActionSpriteFileName, EngineConstants.BasherAnimationFrames);
        SetData(EngineConstants.MinerActionId, EngineConstants.MinerActionName, EngineConstants.MinerActionSpriteFileName, EngineConstants.MinerAnimationFrames);
        SetData(EngineConstants.DiggerActionId, EngineConstants.DiggerActionName, EngineConstants.DiggerActionSpriteFileName, EngineConstants.DiggerAnimationFrames);
        SetData(EngineConstants.PlatformerActionId, EngineConstants.PlatformerActionName, EngineConstants.PlatformerActionSpriteFileName, EngineConstants.PlatformerAnimationFrames);
        SetData(EngineConstants.StackerActionId, EngineConstants.StackerActionName, EngineConstants.StackerActionSpriteFileName, EngineConstants.StackerAnimationFrames);
        SetData(EngineConstants.FencerActionId, EngineConstants.FencerActionName, EngineConstants.FencerActionSpriteFileName, EngineConstants.FencerAnimationFrames);
        SetData(EngineConstants.GliderActionId, EngineConstants.GliderActionName, EngineConstants.GliderActionSpriteFileName, EngineConstants.GliderAnimationFrames);
        SetData(EngineConstants.JumperActionId, EngineConstants.JumperActionName, EngineConstants.JumperActionSpriteFileName, EngineConstants.JumperAnimationFrames);
        SetData(EngineConstants.SwimmerActionId, EngineConstants.SwimmerActionName, EngineConstants.SwimmerActionSpriteFileName, EngineConstants.SwimmerAnimationFrames);
        SetData(EngineConstants.ShimmierActionId, EngineConstants.ShimmierActionName, EngineConstants.ShimmierActionSpriteFileName, EngineConstants.ShimmierAnimationFrames);
        SetData(EngineConstants.LasererActionId, EngineConstants.LasererActionName, EngineConstants.LasererActionSpriteFileName, EngineConstants.LasererAnimationFrames);
        SetData(EngineConstants.SliderActionId, EngineConstants.SliderActionName, EngineConstants.SliderActionSpriteFileName, EngineConstants.SliderAnimationFrames);
        SetData(EngineConstants.FallerActionId, EngineConstants.FallerActionName, EngineConstants.FallerActionSpriteFileName, EngineConstants.FallerAnimationFrames);
        SetData(EngineConstants.AscenderActionId, EngineConstants.AscenderActionName, EngineConstants.AscenderActionSpriteFileName, EngineConstants.AscenderAnimationFrames);
        SetData(EngineConstants.ShruggerActionId, EngineConstants.ShruggerActionName, EngineConstants.ShruggerActionSpriteFileName, EngineConstants.ShruggerAnimationFrames);
        SetData(EngineConstants.DrownerActionId, EngineConstants.DrownerActionName, EngineConstants.DrownerActionSpriteFileName, EngineConstants.DrownerAnimationFrames);
        SetData(EngineConstants.HoisterActionId, EngineConstants.HoisterActionName, EngineConstants.HoisterActionSpriteFileName, EngineConstants.HoisterAnimationFrames);
        SetData(EngineConstants.DehoisterActionId, EngineConstants.DehoisterActionName, EngineConstants.DehoisterActionSpriteFileName, EngineConstants.DehoisterAnimationFrames);
        SetData(EngineConstants.ReacherActionId, EngineConstants.ReacherActionName, EngineConstants.ReacherActionSpriteFileName, EngineConstants.ReacherAnimationFrames);
        SetData(EngineConstants.DisarmerActionId, EngineConstants.DisarmerActionName, EngineConstants.DisarmerActionSpriteFileName, EngineConstants.DisarmerAnimationFrames);
        SetData(EngineConstants.ExiterActionId, EngineConstants.ExiterActionName, EngineConstants.ExiterActionSpriteFileName, EngineConstants.ExiterAnimationFrames);
        SetData(EngineConstants.ExploderActionId, EngineConstants.ExploderActionName, EngineConstants.ExploderActionSpriteFileName, EngineConstants.ExploderAnimationFrames);
        SetData(EngineConstants.OhNoerActionId, EngineConstants.OhNoerActionName, EngineConstants.OhNoerActionSpriteFileName, EngineConstants.OhNoerAnimationFrames);
        SetData(EngineConstants.SplatterActionId, EngineConstants.SplatterActionName, EngineConstants.SplatterActionSpriteFileName, EngineConstants.SplatterAnimationFrames);
        SetData(EngineConstants.StonerActionId, EngineConstants.StonerActionName, EngineConstants.StonerActionSpriteFileName, EngineConstants.StonerAnimationFrames);
        SetData(EngineConstants.VaporiserActionId, EngineConstants.VaporiserActionName, EngineConstants.VaporiserActionSpriteFileName, EngineConstants.VaporiserAnimationFrames);
        SetData(EngineConstants.RotateClockwiseActionId, EngineConstants.RotateClockwiseActionName, EngineConstants.RotateClockwiseActionSpriteFileName, EngineConstants.RotateClockwiseAnimationFrames);
        SetData(EngineConstants.RotateCounterclockwiseActionId, EngineConstants.RotateCounterclockwiseActionName, EngineConstants.RotateCounterclockwiseActionSpriteFileName, EngineConstants.RotateCounterclockwiseAnimationFrames);
        SetData(EngineConstants.RotateHalfActionId, EngineConstants.RotateHalfActionName, EngineConstants.RotateHalfActionSpriteFileName, EngineConstants.RotateHalfAnimationFrames);

        if (count != EngineConstants.NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetData(int index, string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
        {
            result[index] = new LemmingActionLookupData(lemmingActionName, lemmingActionFileName, numberOfAnimationFrames);
            count++;
        }
    }

    public static LemmingActionLookupData GetLemmingActionDataFromId(int lemmingActionId) => LemmingActionIdToStringLookup[lemmingActionId];

    public readonly struct LemmingActionLookupData(string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
    {
        public readonly string LemmingActionName = lemmingActionName;
        public readonly string LemmingActionFileName = lemmingActionFileName;
        public readonly int NumberOfAnimationFrames = numberOfAnimationFrames;
    }

    #endregion
}
