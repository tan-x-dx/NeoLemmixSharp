using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath, GraphicsDevice graphicsDevice)
    {
        var levelData = new LevelData();

        var charEqualityComparer = new CaseInvariantCharEqualityComparer();

        var terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();

        var levelDataReader = new LevelDataReader(levelData);
        var skillSetReader = new SkillSetReader(charEqualityComparer);
        var terrainGroupReader = new TerrainGroupReader(terrainArchetypes);
        var gadgetReader = new GadgetReader(charEqualityComparer);
        var talismanReader = new TalismanReader(charEqualityComparer);

        var dataReaders = new INeoLemmixDataReader[]
        {
            levelDataReader,
            skillSetReader,
            terrainGroupReader,
            new TerrainReader(terrainArchetypes, levelData.AllTerrainData),
            new LemmingReader(levelData.PrePlacedLemmingData),
            gadgetReader,
            talismanReader,
            new NeoLemmixTextReader(levelData.PreTextLines, "$PRETEXT"),
            new NeoLemmixTextReader(levelData.PostTextLines, "$POSTTEXT"),
            new SketchReader(levelData.AllSketchData),
        };

        var dataReaderList = new DataReaderList(dataReaders);

        dataReaderList.ReadFile(levelFilePath);

        ProcessLevelData(levelData, levelDataReader, talismanReader, skillSetReader);
        ProcessTerrainData(levelData, terrainGroupReader, terrainArchetypes);
        ProcessGadgetData(levelData, graphicsDevice, gadgetReader);
        ProcessConfigData(levelData);

        return levelData;
    }

    public string ScrapeLevelTitle(string levelFilePath)
    {
        using var stream = new FileStream(levelFilePath, FileMode.Open);
        using var streamReader = new StreamReader(stream);

        string? levelTitle = null;

        while (streamReader.ReadLine() is { } line)
        {
            if (ReadingHelpers.LineIsBlankOrComment(line))
                continue;

            if (LevelDataReader.TryReadLevelTitle(line, out levelTitle))
            {
                break;
            }
        }

        return string.IsNullOrWhiteSpace(levelTitle)
            ? "Untitled"
            : levelTitle;
    }

    private static void ProcessLevelData(
        LevelData levelData,
        LevelDataReader levelDataReader,
        TalismanReader talismanReader,
        SkillSetReader skillSetReader)
    {
        var objectiveRequirementsList = new List<IObjectiveRequirement>
        {
            new BasicSkillSetRequirement(skillSetReader.SkillSetData.ToArray()),
            new SaveRequirement(levelDataReader.SaveRequirement)
        };

        if (levelDataReader.TimeLimitInSeconds.HasValue)
        {
            objectiveRequirementsList.Add(new TimeRequirement(levelDataReader.TimeLimitInSeconds.Value));
        }

        levelData.LevelObjectives.Capacity = 1 + talismanReader.TalismanData.Count;
        levelData.LevelObjectives.Add(new LevelObjective(
            0,
            "Save Lemmings",
            objectiveRequirementsList.ToArray()));

        ProcessTalismans(levelData, talismanReader);
    }

    private static void ProcessTalismans(
        LevelData levelData,
        TalismanReader talismanReader)
    {
        foreach (var talismanDatum in talismanReader.TalismanData)
        {
            levelData.LevelObjectives.Add(talismanDatum.ToLevelObjective(levelData));
        }
    }

    private static void ProcessTerrainData(
        LevelData levelData,
        TerrainGroupReader terrainGroupReader,
        Dictionary<string, TerrainArchetypeData> terrainArchetypes)
    {
        levelData.TerrainArchetypeData.Capacity = terrainArchetypes.Count;
        levelData.TerrainArchetypeData.AddRange(terrainArchetypes.Values.OrderBy(d => d.TerrainArchetypeId));

        levelData.AllTerrainGroups.AddRange(terrainGroupReader.AllTerrainGroups);
    }

    private static void ProcessGadgetData(
        LevelData levelData,
        GraphicsDevice graphicsDevice,
        GadgetReader gadgetReader)
    {
        CalculateHatchCounts(levelData, gadgetReader);

        new GadgetTranslator(levelData, graphicsDevice)
            .TranslateNeoLemmixGadgets(gadgetReader.GadgetArchetypes, gadgetReader.AllGadgetData);
    }

    private static void CalculateHatchCounts(LevelData levelData, GadgetReader gadgetReader)
    {
        var gadgetDataSpan = CollectionsMarshal.AsSpan(gadgetReader.AllGadgetData);
        var hatchGadgets = new List<NeoLemmixGadgetData>();

        foreach (var gadgetData in gadgetDataSpan)
        {
            if (GadgetDataIsHatch(gadgetReader.GadgetArchetypes, gadgetData))
            {
                hatchGadgets.Add(gadgetData);
            }
        }

        const int maxStackallocSize = 64;

        var maxLemmingCountFromHatches = 0;
        var hasInfiniteHatchCount = false;
        var spanLength = hatchGadgets.Count * 2;
        Span<int> countSpan = spanLength > maxStackallocSize
            ? new int[spanLength]
            : stackalloc int[spanLength];

        var i = 0;
        while (i < hatchGadgets.Count)
        {
            ref var runningCount = ref countSpan[i * 2];
            ref var maxCount = ref countSpan[i * 2 + 1];

            runningCount = 0;
            var correspondingHatchGadget = hatchGadgets[i];
            maxCount = correspondingHatchGadget.LemmingCount ?? -1;
            hasInfiniteHatchCount |= !correspondingHatchGadget.LemmingCount.HasValue;
            maxLemmingCountFromHatches += correspondingHatchGadget.LemmingCount ?? 0;

            i++;
        }

        var numberOfLemmingsFromHatches = CalculateTotalHatchLemmingCounts(
            levelData,
            hasInfiniteHatchCount ? null : maxLemmingCountFromHatches);

        var numberOfLemmingsAssignedToHatches = 0;
        i = 0;
        while (numberOfLemmingsAssignedToHatches < numberOfLemmingsFromHatches)
        {
            ref var runningCount = ref countSpan[i * 2];
            var maxCount = countSpan[i * 2 + 1];

            if (maxCount == -1 || runningCount < maxCount)
            {
                runningCount++;
                numberOfLemmingsAssignedToHatches++;
            }

            i++;
            if (i == hatchGadgets.Count)
            {
                i = 0;
            }
        }

        i = 0;
        while (i < hatchGadgets.Count)
        {
            var hatchCount = countSpan[i * 2];
            var correspondingHatchGadget = hatchGadgets[i];
            correspondingHatchGadget.LemmingCount = hatchCount;
            i++;
        }
    }

    private static bool GadgetDataIsHatch(
        Dictionary<string, NeoLemmixGadgetArchetypeData> gadgetArchetypes,
        NeoLemmixGadgetData gadgetData)
    {
        var gadgetArchetypeId = gadgetData.GadgetArchetypeId;

        var gadgetArchetypeData = gadgetArchetypes
            .Values
            .First(a => a.GadgetArchetypeId == gadgetArchetypeId);

        return gadgetArchetypeData.Behaviour == NeoLemmixGadgetBehaviour.Entrance;
    }

    private static int CalculateTotalHatchLemmingCounts(
        LevelData levelData,
        int? maxLemmingCountFromHatches)
    {
        var hatchLemmingData = levelData.HatchLemmingData;
        var hatchLemmingCount = levelData.NumberOfLemmings - levelData.PrePlacedLemmingData.Count;

        var totalNumberOfHatchLemmings = maxLemmingCountFromHatches.HasValue
            ? Math.Min(hatchLemmingCount, maxLemmingCountFromHatches.Value)
            : hatchLemmingCount;

        hatchLemmingData.Capacity = totalNumberOfHatchLemmings;
        levelData.NumberOfLemmings = totalNumberOfHatchLemmings;

        while (hatchLemmingData.Count < hatchLemmingData.Capacity)
        {
            hatchLemmingData.Add(new LemmingData());
        }

        return totalNumberOfHatchLemmings;
    }

    private static void ProcessConfigData(LevelData levelData)
    {
        var levelParameters = levelData.LevelParameters;

        // Add all default parameters for a NeoLemmix level
        levelParameters.Add(LevelParameters.EnablePause);
        levelParameters.Add(LevelParameters.EnableNuke);
        levelParameters.Add(LevelParameters.EnableFastForward);
        levelParameters.Add(LevelParameters.EnableDirectionSelect);
        levelParameters.Add(LevelParameters.EnableClearPhysics);
        levelParameters.Add(LevelParameters.EnableSkillShadows);
        levelParameters.Add(LevelParameters.EnableFrameControl);

        var controlPanelParameters = levelData.ControlParameters;

        controlPanelParameters.Add(ControlPanelParameters.ShowPauseButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowNukeButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowFastForwardsButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowRestartButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowFrameNudgeButtons);
        controlPanelParameters.Add(ControlPanelParameters.ShowDirectionSelectButtons);
        controlPanelParameters.Add(ControlPanelParameters.ShowClearPhysicsAndReplayButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowReleaseRateButtonsIfPossible);
        controlPanelParameters.Add(ControlPanelParameters.EnableClassicModeSkillsIfPossible);
        controlPanelParameters.Add(ControlPanelParameters.RemoveSkillAssignPaddingButtons);
    }

    public void Dispose()
    {
    }
}
