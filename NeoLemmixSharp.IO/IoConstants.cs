namespace NeoLemmixSharp.IO;

public static class IoConstants
{
    /// <summary>
    /// Assumption: if there are infinite skills available of a certain type,
    /// there'll probably be around this number of actual usages.
    /// </summary>
    public const int AssumedSkillUsageForInfiniteSkillCounts = 40;
    /// <summary>
    /// Assumption: if there are skill pickups in a level,
    /// there'll probably be around this number of skills added.
    /// </summary>
    public const int AssumedSkillCountsFromPickups = 10;

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
}
