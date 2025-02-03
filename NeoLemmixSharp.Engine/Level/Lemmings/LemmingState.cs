using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Teams;
using System.Numerics;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState : ISnapshotDataConvertible<LemmingStateSnapshotData>
{
    private readonly Lemming _lemming;
    private int _teamId;

    private uint _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color FootColor { get; private set; }
    public Color BodyColor { get; private set; }

    public bool HasPermanentSkill => (_states & EngineConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states & EngineConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states & EngineConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_states & EngineConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states & EngineConstants.AssignableSkillBitMask) == (1U << EngineConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states >> EngineConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.ClimberBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_states >> EngineConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.FloaterBitIndex;
                _states &= ~(1U << EngineConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                _states &= ~(1U << EngineConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states >> EngineConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.GliderBitIndex;
                _states &= ~(1U << EngineConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                _states &= ~(1U << EngineConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states >> EngineConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.SliderBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_states >> EngineConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.SwimmerBitIndex;
                _states &= ~((1U << EngineConstants.AcidLemmingBitIndex) | (1U << EngineConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                _states &= ~(1U << EngineConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_states >> EngineConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.DisarmerBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsNeutral
    {
        get => ((_states >> EngineConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.NeutralBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_states >> EngineConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.PermanentFastForwardBitIndex);
            }
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states >> EngineConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.AcidLemmingBitIndex;
                _states &= ~((1U << EngineConstants.SwimmerBitIndex) | (1U << EngineConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                _states &= ~(1U << EngineConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_states >> EngineConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.WaterLemmingBitIndex;
                _states &= ~((1U << EngineConstants.SwimmerBitIndex) | (1U << EngineConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                _states &= ~(1U << EngineConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_states >> EngineConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << EngineConstants.ActiveBitIndex;
            }
            else
            {
                _states &= ~(1U << EngineConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsZombie
    {
        get => ((_states >> EngineConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            if (value)
            {
                _states |= 1U << EngineConstants.ZombieBitIndex;
                LevelScreen.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                _states &= ~(1U << EngineConstants.ZombieBitIndex);
                LevelScreen.LemmingManager.DeregisterZombie(_lemming);
            }
            UpdateSkinColor();
        }
    }

    public Team TeamAffiliation
    {
        get => LevelScreen.TeamManager.AllItems[_teamId];
        set
        {
            _teamId = value.Id;
            UpdateHairAndBodyColors();
            UpdateSkinColor();
        }
    }

    public LemmingState(Lemming lemming, int teamId)
    {
        _lemming = lemming;
        _teamId = teamId;
    }

    private void UpdateHairAndBodyColors()
    {
        var team = LevelScreen.TeamManager.AllItems[_teamId];

        if (HasPermanentSkill)
        {
            HairColor = team.PermanentSkillHairColor;
            BodyColor = IsNeutral
                ? team.NeutralBodyColor
                : team.PermanentSkillBodyColor;
        }
        else
        {
            HairColor = team.HairColor;
            BodyColor = IsNeutral
                ? team.NeutralBodyColor
                : team.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        var team = LevelScreen.TeamManager.AllItems[_teamId];

        SkinColor = IsZombie
            ? team.ZombieSkinColor
            : team.SkinColor;

        if (IsAcidLemming)
        {
            FootColor = team.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            FootColor = team.WaterLemmingFootColor;
        }
        else
        {
            FootColor = SkinColor;
        }
    }

    public void SetRawDataFromOther(LemmingState otherLemmingState)
    {
        _teamId = otherLemmingState._teamId;
        _states = otherLemmingState._states;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    public void SetRawDataFromOther(uint rawData)
    {
        _states = rawData;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    public void SetFromSnapshotData(in LemmingStateSnapshotData lemmingStateSnapshotData)
    {
        _teamId = lemmingStateSnapshotData.TeamId;
        _states = lemmingStateSnapshotData.StateData;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    public void WriteToSnapshotData(out LemmingStateSnapshotData data)
    {
        data = new LemmingStateSnapshotData(_teamId, _states);
    }
}