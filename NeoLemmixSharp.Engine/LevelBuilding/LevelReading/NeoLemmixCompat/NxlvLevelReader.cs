using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly LevelData _levelData;

    private readonly LevelDataReader _levelDataReader;
    private readonly SkillSetReader _skillSetReader;
    private readonly TerrainGroupReader _terrainGroupReader;
    private readonly GadgetReader _gadgetReader;
    private readonly TalismanReader _talismanReader;

    private readonly DataReaderList _dataReaderList;

    public NxlvLevelReader(string filePath)
    {
        _levelData = new LevelData();
        var uniqueStringSet = new UniqueStringSet();

        _levelDataReader = new LevelDataReader(uniqueStringSet, _levelData);
        _skillSetReader = new SkillSetReader();
        _terrainGroupReader = new TerrainGroupReader(uniqueStringSet);
        _gadgetReader = new GadgetReader(uniqueStringSet);
        _talismanReader = new TalismanReader(uniqueStringSet);

        var dataReaders = new NeoLemmixDataReader[]
        {
            _levelDataReader,
            _skillSetReader,
            _terrainGroupReader,
            new TerrainReader(uniqueStringSet, _levelData.AllTerrainData),
            new LemmingReader(_levelData.PrePlacedLemmingData),
            _gadgetReader,
            _talismanReader,
            new NeoLemmixTextReader(uniqueStringSet,_levelData.PreTextLines, "$PRETEXT"),
            new NeoLemmixTextReader(uniqueStringSet,_levelData.PostTextLines, "$POSTTEXT"),
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

        NxlvCountHelpers.CalculateHatchCounts(_levelData, _levelDataReader, _gadgetReader);

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
        //  _levelData.TerrainArchetypeData.Capacity = _terrainArchetypes.Count;
        //  _levelData.TerrainArchetypeData.AddRange(_terrainArchetypes.Values.OrderBy(d => d.TerrainArchetypeId));

        _levelData.AllTerrainGroups.AddRange(_terrainGroupReader.AllTerrainGroups);
    }

    private void ProcessGadgetData(
        GraphicsDevice graphicsDevice)
    {
        //  new GadgetTranslator(_levelData, graphicsDevice)
        //      .TranslateNeoLemmixGadgets(_gadgetReader.GadgetArchetypes, _gadgetReader.AllGadgetData);
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
}
