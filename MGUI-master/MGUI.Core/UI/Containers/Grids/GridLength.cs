using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MGUI.Core.UI.Containers.Grids;

/// <summary>Describes the kind of value that a <see cref="GridLength"/> object is holding.</summary>
public enum GridUnitType
{
    /// <summary>The size is determined by the size properties of the content object.</summary>
    Auto = 0,
    /// <summary>The value is expressed as a pixel.</summary>
    Pixel = 1,
    /// <summary>The value is expressed as a weighted proportion of available space.</summary>
    Weighted = 2
}

/// <summary>To instantiate this struct, use:<para/>
/// <see cref="Auto"/><br/>
/// <see cref="CreatePixelLength(int)"/><br/>
/// <see cref="CreateWeightedLength(double)"/>.<para/>
/// <see cref="Parse(string)"/><br/>
/// <see cref="ParseMultiple(string)"/></summary>
[TypeConverter(typeof(GridLengthStringConverter))]
public readonly struct GridLength : IEquatable<GridLength>
{
    public readonly GridUnitType UnitType;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsPixelLength => UnitType == GridUnitType.Pixel;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsAutoLength => UnitType == GridUnitType.Auto;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsWeightedLength => UnitType == GridUnitType.Weighted;

    private readonly int? _pixels;
    /// <summary>Only valid if <see cref="IsPixelLength"/> is true</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Pixels => IsPixelLength ? _pixels!.Value : throw new InvalidOperationException($"{nameof(GridLength)}.{nameof(Pixels)} is only available when {nameof(UnitType)} equals {nameof(GridUnitType)}.{nameof(GridUnitType.Pixel)}");

    private readonly double? _weight;
    /// <summary>Only valid if <see cref="IsWeightedLength"/> is true</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public double Weight => IsWeightedLength ? _weight!.Value : throw new InvalidOperationException($"{nameof(GridLength)}.{nameof(Weight)} is only available when {nameof(UnitType)} equals {nameof(GridUnitType)}.{nameof(GridUnitType.Weighted)}");

    private GridLength(GridUnitType unitType, int? pixels, double? weight)
    {
        _pixels = pixels;
        _weight = weight;

        if (IsPixelLength && !pixels.HasValue)
            throw new ArgumentNullException(nameof(pixels));
        if (IsWeightedLength && !weight.HasValue)
            throw new ArgumentNullException(nameof(weight));
    }

    public static readonly GridLength Auto = new(GridUnitType.Auto, null, null);
    public static GridLength CreatePixelLength(int pixels) => new(GridUnitType.Pixel, pixels, null);
    public static GridLength CreateWeightedLength(double weight) => new(GridUnitType.Weighted, null, weight);

    public static bool operator ==(GridLength gl1, GridLength gl2)
        => gl1.UnitType == gl2.UnitType && gl1._pixels == gl2._pixels && gl1._weight == gl2._weight;
    public static bool operator !=(GridLength gl1, GridLength gl2)
        => gl1.UnitType != gl2.UnitType || gl1._pixels != gl2._pixels || gl1._weight != gl2._weight;

    public override bool Equals(object other)
    {
        if (other is GridLength gl)
            return this == gl;

        return false;
    }
    public bool Equals(GridLength other) => this == other;

    public override int GetHashCode() => (UnitType, _pixels ?? _weight ?? 0).GetHashCode();

    public override string ToString() => UnitType switch
    {
        GridUnitType.Auto => "Auto",
        GridUnitType.Pixel => $"{Pixels}px",
        GridUnitType.Weighted => $"{Weight:0.##}*",
        _ => throw new NotImplementedException($"Unrecognized {nameof(GridUnitType)}: {UnitType}")
    };

    private const string AutoLengthPattern = "(?<AutoLength>(?i)auto(?-i))";
    private const string PixelLengthPattern = @"(?<PixelLength>(?<PixelValue>\d+)((?i)px(?-i))?)";
    private const string WeightedLengthPattern = @"(?<WeightedLength>(?<WeightValue>(\d+(\.\d+)?)?)\*)";
    private const string DimensionLengthPattern = $"({AutoLengthPattern}|{WeightedLengthPattern}|{PixelLengthPattern})";

    /// <summary>A <see cref="Regex"/> that parses <see cref="GridLength"/>s from strings.<br/>
    /// Does not include start of string ('^') or end of string ('$') regex anchors.<para/>
    /// See also: <see cref="AnchoredParser"/><para/>
    /// EX: "Auto,16px,25,*,1.5*,Auto" would contain 6 Matches</summary>
    public static readonly Regex UnanchoredParser = new(DimensionLengthPattern);
    /// <summary>A <see cref="Regex"/> that parses <see cref="GridLength"/>s from strings.<br/>
    /// Include start of string ('^') and end of string ('$') regex anchors.<para/>
    /// See also: <see cref="UnanchoredParser"/><para/>
    /// EX: "1.5*" would result in a weighted <see cref="GridLength"/> with Weight=1.5.<br/>
    /// But "1.5*,1.2*" would not match because the start of string and end of string anchors only allow 1 <see cref="GridLength"/> match within the string.</summary>
    public static readonly Regex AnchoredParser = new($@"^{DimensionLengthPattern}$");

    /// <param name="length">To represent a <see cref="GridUnitType.Auto"/> dimension: "Auto" (case-insensitive)<br/>
    /// To represent a <see cref="GridUnitType.Pixel"/> dimension: A positive integral value. EX: "50"<br/>
    /// To represent a <see cref="GridUnitType.Weighted"/> dimension: A positive numeric value, suffixed with '*'. EX: "1.5*". If no numeric value is present, it is assumed to be "1*"</param>
    public static GridLength Parse(string length)
    {
        var match = AnchoredParser.Match(length);
        if (match.Groups["AutoLength"].Success)
            return Auto;

        if (match.Groups["PixelLength"].Success)
        {
            var pixelValueString = match.Groups["PixelValue"].Value;
            var pixels = int.Parse(pixelValueString);
            return CreatePixelLength(pixels);
        }

        if (match.Groups["WeightedLength"].Success)
        {
            var weightValueString = match.Groups["WeightValue"].Value;
            var weight = weightValueString == string.Empty ? 1.0 : double.Parse(weightValueString);
            return CreateWeightedLength(weight);
        }

        throw new NotImplementedException($"Unrecognized {nameof(GridLength)} value: {length}");
    }

    /// <param name="commaSeparatedValues">A comma-separated list of dimensions, where each value is either:<para/>
    /// To represent a <see cref="GridUnitType.Auto"/> dimension: "Auto" (case-insensitive)<br/>
    /// To represent a <see cref="GridUnitType.Pixel"/> dimension: A positive integral value. EX: "50"<br/>
    /// To represent a <see cref="GridUnitType.Weighted"/> dimension: A positive numeric value, suffixed with '*'. EX: "1.5*". If no numeric value is present, it is assumed to be "1*"<para/>
    /// EX: "1.25*,1*,200,Auto" would yield 4 <see cref="GridLength"/>s.</param>
    public static IEnumerable<GridLength> ParseMultiple(string commaSeparatedValues)
    {
        if (!string.IsNullOrEmpty(commaSeparatedValues))
        {
            foreach (var item in commaSeparatedValues.Split(','))
                yield return Parse(item);
        }
    }
}

public sealed class GridLengthStringConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            var length = GridLength.Parse(stringValue);
            return length;
        }

        return base.ConvertFrom(context, culture, value);
    }
}