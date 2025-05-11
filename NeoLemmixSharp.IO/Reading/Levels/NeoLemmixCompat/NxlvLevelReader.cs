using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers.TerrainReaders;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat;

internal sealed class NxlvLevelReader : ILevelReader
{
    private readonly LevelData _levelData;
    private readonly UniqueStringSet _uniqueStringSet = new();

    private readonly LevelDataReader _levelDataReader;
    private readonly SkillSetReader _skillSetReader;
    private readonly TerrainGroupReader _terrainGroupReader;
    private readonly GadgetReader _gadgetReader;
    private readonly TalismanReader _talismanReader;

    private readonly DataReaderList _dataReaderList;

    public NxlvLevelReader(string filePath)
    {
        _levelData = new LevelData();

        _levelDataReader = new LevelDataReader(_uniqueStringSet, _levelData);
        _skillSetReader = new SkillSetReader();
        _terrainGroupReader = new TerrainGroupReader(_uniqueStringSet);
        _gadgetReader = new GadgetReader(_uniqueStringSet);
        _talismanReader = new TalismanReader(_uniqueStringSet);

        var dataReaders = new NeoLemmixDataReader[]
        {
            _levelDataReader,
            _skillSetReader,
            _terrainGroupReader,
            new TerrainReader(_uniqueStringSet, _levelData.AllTerrainData),
            new LemmingReader(_levelData.PrePlacedLemmingData),
            _gadgetReader,
            _talismanReader,
            new NeoLemmixTextReader(_uniqueStringSet, _levelData.PreTextLines, "$PRETEXT"),
            new NeoLemmixTextReader(_uniqueStringSet, _levelData.PostTextLines, "$POSTTEXT"),
            new SketchReader(_levelData.AllSketchData),
        };

        _dataReaderList = new DataReaderList(filePath, dataReaders);
    }

    public LevelData ReadLevel(GraphicsDevice graphicsDevice)
    {
        _dataReaderList.ReadFile();

        ProcessLevelData();
        ProcessTalismans();

        // NeoLemmixStyleHelpers.ProcessTerrainArchetypeData(_levelData);
        // NeoLemmixStyleHelpers.ProcessGadgetArchetypeData(_levelData, _uniqueStringSet);

        NxlvCountHelpers.CalculateHatchCounts(_levelData, _levelDataReader, _gadgetReader);

        ProcessConfigData();

        _levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(_levelData);
        _levelData.AllTerrainGroups.AddRange(_terrainGroupReader.AllTerrainGroups);

        return _levelData;
    }

    private void ProcessLevelData()
    {
        _levelData.NumberOfTeams = 1;

        /*  var objectiveRequirementsList = new List<IObjectiveRequirement>
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
              objectiveRequirementsList.ToArray()));*/
    }

    private void ProcessTalismans()
    {
        /*   foreach (var talismanDatum in _talismanReader.TalismanData)
           {
               _levelData.LevelObjectives.Add(talismanDatum.ToLevelObjective(_levelData));
           }*/
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
