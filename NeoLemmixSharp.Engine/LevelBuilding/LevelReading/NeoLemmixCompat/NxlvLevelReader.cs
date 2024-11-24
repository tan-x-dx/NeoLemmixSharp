using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader, IEqualityComparer<char>
{
    private readonly LevelData _levelData;
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    private readonly LevelDataReader _levelDataReader;
    private readonly SkillSetReader _skillSetReader;
    private readonly TerrainGroupReader _terrainGroupReader;
    private readonly GadgetReader _gadgetReader;
    private readonly TalismanReader _talismanReader;

    private readonly DataReaderList _dataReaderList;

    public NxlvLevelReader(string filePath)
    {
        _levelData = new LevelData();
        _terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();

        _levelDataReader = new LevelDataReader(_levelData);
        _skillSetReader = new SkillSetReader(this);
        _terrainGroupReader = new TerrainGroupReader(_terrainArchetypes);
        _gadgetReader = new GadgetReader(this);
        _talismanReader = new TalismanReader(this);

        var dataReaders = new INeoLemmixDataReader[]
        {
            _levelDataReader,
            _skillSetReader,
            _terrainGroupReader,
            new TerrainReader(_terrainArchetypes, _levelData.AllTerrainData),
            new LemmingReader(_levelData.PrePlacedLemmingData),
            _gadgetReader,
            _talismanReader,
            new NeoLemmixTextReader(_levelData.PreTextLines, "$PRETEXT"),
            new NeoLemmixTextReader(_levelData.PostTextLines, "$POSTTEXT"),
            new SketchReader(_levelData.AllSketchData),
        };

        _dataReaderList = new DataReaderList(filePath, dataReaders);
    }

    public LevelData ReadLevel(GraphicsDevice graphicsDevice)
    {
        _dataReaderList.ReadFile();

        ProcessLevelData();
        ProcessTalismans();
        ProcessTerrainData();
        ProcessGadgetData(graphicsDevice);
        ProcessConfigData();

        _levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(_levelData);

        return _levelData;
    }

    private void ProcessLevelData()
    {
        var objectiveRequirementsList = new List<IObjectiveRequirement>
        {
            new BasicSkillSetRequirement(_skillSetReader.SkillSetData.ToArray()),
            new SaveRequirement(_levelDataReader.SaveRequirement)
        };

        if (_levelDataReader.TimeLimitInSeconds.HasValue)
        {
            objectiveRequirementsList.Add(new TimeRequirement(_levelDataReader.TimeLimitInSeconds.Value));
        }

        _levelData.LevelObjectives.Capacity = 1 + _talismanReader.TalismanData.Count;
        _levelData.LevelObjectives.Add(new LevelObjective(
            0,
            "Save Lemmings",
            objectiveRequirementsList.ToArray()));
    }

    private void ProcessTalismans()
    {
        foreach (var talismanDatum in _talismanReader.TalismanData)
        {
            _levelData.LevelObjectives.Add(talismanDatum.ToLevelObjective(_levelData));
        }
    }

    private void ProcessTerrainData()
    {
        _levelData.TerrainArchetypeData.Capacity = _terrainArchetypes.Count;
        _levelData.TerrainArchetypeData.AddRange(_terrainArchetypes.Values.OrderBy(d => d.TerrainArchetypeId));

        _levelData.AllTerrainGroups.AddRange(_terrainGroupReader.AllTerrainGroups);
    }

    private void ProcessGadgetData(
        GraphicsDevice graphicsDevice)
    {
        CalculateHatchCounts();

        new GadgetTranslator(_levelData, graphicsDevice)
            .TranslateNeoLemmixGadgets(_gadgetReader.GadgetArchetypes, _gadgetReader.AllGadgetData);
    }

    private void CalculateHatchCounts()
    {
        var gadgetDataSpan = CollectionsMarshal.AsSpan(_gadgetReader.AllGadgetData);
        var hatchGadgets = new List<NeoLemmixGadgetData>();

        foreach (var gadgetData in gadgetDataSpan)
        {
            if (GadgetDataIsHatch(_gadgetReader.GadgetArchetypes, gadgetData))
            {
                hatchGadgets.Add(gadgetData);
            }
        }

        const int maxStackallocSize = 64;

        var maxLemmingCountFromHatches = 0;
        var hasInfiniteHatchCount = false;
        var spanLength = hatchGadgets.Count;
        Span<HatchCountData> countSpan = spanLength > maxStackallocSize
            ? new HatchCountData[spanLength]
            : stackalloc HatchCountData[spanLength];

        var i = 0;
        while (i < countSpan.Length)
        {
            var correspondingHatchGadget = hatchGadgets[i];
            countSpan[i].RunningCount = 0;
            countSpan[i].MaxCount = correspondingHatchGadget.LemmingCount ?? -1;
            hasInfiniteHatchCount |= !correspondingHatchGadget.LemmingCount.HasValue;
            maxLemmingCountFromHatches += correspondingHatchGadget.LemmingCount ?? 0;

            i++;
        }

        var numberOfLemmingsFromHatches = CalculateTotalHatchLemmingCounts(
            _levelData,
            _levelDataReader,
            hasInfiniteHatchCount ? null : maxLemmingCountFromHatches);

        var numberOfLemmingsAssignedToHatches = 0;
        i = 0;
        while (numberOfLemmingsAssignedToHatches < numberOfLemmingsFromHatches)
        {
            ref var runningCount = ref countSpan[i].RunningCount;
            var maxCount = countSpan[i].MaxCount;

            if (maxCount == -1 || runningCount < maxCount)
            {
                runningCount++;
                numberOfLemmingsAssignedToHatches++;
            }

            i++;
            if (i == countSpan.Length)
            {
                i = 0;
            }
        }

        i = 0;
        while (i < countSpan.Length)
        {
            var hatchCount = countSpan[i].RunningCount;
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
        LevelDataReader levelDataReader,
        int? maxLemmingCountFromHatches)
    {
        var hatchLemmingData = levelData.HatchLemmingData;
        var hatchLemmingCount = levelDataReader.NumberOfLemmings - levelData.PrePlacedLemmingData.Count;

        var totalNumberOfHatchLemmings = maxLemmingCountFromHatches.HasValue
            ? Math.Min(hatchLemmingCount, maxLemmingCountFromHatches.Value)
            : hatchLemmingCount;

        hatchLemmingData.Capacity = totalNumberOfHatchLemmings;

        while (hatchLemmingData.Count < hatchLemmingData.Capacity)
        {
            hatchLemmingData.Add(new LemmingData());
        }

        return totalNumberOfHatchLemmings;
    }

    private void ProcessConfigData()
    {
        var levelParameters = _levelData.LevelParameters;

        // Add all default parameters for a NeoLemmix level
        levelParameters.Add(LevelParameters.EnablePause);
        levelParameters.Add(LevelParameters.EnableNuke);
        levelParameters.Add(LevelParameters.EnableFastForward);
        levelParameters.Add(LevelParameters.EnableDirectionSelect);
        levelParameters.Add(LevelParameters.EnableClearPhysics);
        levelParameters.Add(LevelParameters.EnableSkillShadows);
        levelParameters.Add(LevelParameters.EnableFrameControl);

        var controlPanelParameters = _levelData.ControlParameters;

        controlPanelParameters.Add(ControlPanelParameters.ShowPauseButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowNukeButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowFastForwardsButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowRestartButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowFrameNudgeButtons);
        controlPanelParameters.Add(ControlPanelParameters.ShowDirectionSelectButtons);
        controlPanelParameters.Add(ControlPanelParameters.ShowClearPhysicsAndReplayButton);
        controlPanelParameters.Add(ControlPanelParameters.ShowSpawnIntervalButtonsIfPossible);
        controlPanelParameters.Add(ControlPanelParameters.EnableClassicModeSkillsIfPossible);
        controlPanelParameters.Add(ControlPanelParameters.RemoveSkillAssignPaddingButtons);
    }

    public void Dispose()
    {
        _dataReaderList.Dispose();
    }

    private struct HatchCountData
    {
        public int RunningCount;
        public int MaxCount;
    }

    bool IEqualityComparer<char>.Equals(char x, char y)
    {
        return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }

    int IEqualityComparer<char>.GetHashCode(char obj)
    {
        return char.ToUpperInvariant(obj).GetHashCode();
    }
}
