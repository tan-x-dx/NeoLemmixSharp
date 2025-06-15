﻿using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO;

public static class IoConstants
{
    internal const byte PeriodByte = (byte)'.';

    internal const int InitialStringListCapacity = 32;

    internal const long MaxAllowedFileSizeInBytes = 1024 * 1024 * 64;
    internal const string FileSizeTooLargeExceptionMessage = "File too large! Max file size is 64Mb";

    internal const string DefaultStyleIdentifierString = "default";
    internal const string DefaultStyleName = "Default";
    internal const string DefaultStyleAuthor = "";
    internal const string DefaultStyleDescription = "A default style containing most basic functionality.";

    internal static readonly StyleIdentifier DefaultStyleIdentifier = new(DefaultStyleIdentifierString);
    internal static readonly StyleFormatPair DefaultStyleFormatPair = new(DefaultStyleIdentifier, FileFormatType.Default);

    /// <summary>
    /// Assumption: if there are infinite skills available of a certain type,
    /// there'll probably be around this number of actual usages.
    /// </summary>
    public const int AssumedSkillUsageForInfiniteSkillCounts = 40;

    /// <summary>
    /// If a style has not been used for this many levels, remove it from the cache
    /// </summary>
    public const int NumberOfLevelsToKeepStyle = 4;

    /// <summary>
    /// Assumption: a level will probably depend on this number or fewer styles
    /// </summary>
    public const int AssumedInitialStyleCapacity = 6;

    /// <summary>
    /// Assumption: a level will probably have this number of unique terrain pieces or fewer
    /// </summary>
    public const int AssumedNumberOfTerrainArchetypeDataInLevel = 32;

    /// <summary>
    /// Assumption: a level will probably have this number of unique gadgets or fewer
    /// </summary>
    public const int AssumedNumberOfGadgetArchetypeDataInLevel = 20;

    /// <summary>
    /// Assumption: a style will probably define this number of unique terrain pieces or fewer
    /// </summary>
    public const int AssumedNumberOfTerrainArchetypeDataInStyle = 64;

    /// <summary>
    /// Assumption: a style will probably define this number of unique gadgets or fewer
    /// </summary>
    public const int AssumedNumberOfGadgetArchetypeDataInStyle = 16;

    public const int MaxStringLengthInBytes = 2048;
}
