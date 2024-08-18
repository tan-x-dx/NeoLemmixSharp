using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    private readonly Lemming _lemming;
    private Team _team;

    private uint _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color FootColor { get; private set; }
    public Color BodyColor { get; private set; }

    public bool HasPermanentSkill => (_states & LevelConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states & LevelConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states & LevelConstants.SpecialFallingBehaviourBitMask) != 0U;

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states & LevelConstants.AssignableSkillBitMask) == (1U << LevelConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states >> LevelConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.ClimberBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_states >> LevelConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.FloaterBitIndex;
                _states &= ~(1U << LevelConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                _states &= ~(1U << LevelConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states >> LevelConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.GliderBitIndex;
                _states &= ~(1U << LevelConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                _states &= ~(1U << LevelConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states >> LevelConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.SliderBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_states >> LevelConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.SwimmerBitIndex;
                _states &= ~((1U << LevelConstants.AcidLemmingBitIndex) | (1U << LevelConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                _states &= ~(1U << LevelConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_states >> LevelConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.DisarmerBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsNeutral
    {
        get => ((_states >> LevelConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.NeutralBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_states >> LevelConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.PermanentFastForwardBitIndex);
            }
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states >> LevelConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.AcidLemmingBitIndex;
                _states &= ~((1U << LevelConstants.SwimmerBitIndex) | (1U << LevelConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                _states &= ~(1U << LevelConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_states >> LevelConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.WaterLemmingBitIndex;
                _states &= ~((1U << LevelConstants.SwimmerBitIndex) | (1U << LevelConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                _states &= ~(1U << LevelConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_states >> LevelConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LevelConstants.ActiveBitIndex;
            }
            else
            {
                _states &= ~(1U << LevelConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsZombie
    {
        get => ((_states >> LevelConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            if (value)
            {
                _states |= 1U << LevelConstants.ZombieBitIndex;
                LevelScreen.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                _states &= ~(1U << LevelConstants.ZombieBitIndex);
                LevelScreen.LemmingManager.DeregisterZombie(_lemming);
            }
            UpdateSkinColor();
        }
    }

    public Team TeamAffiliation
    {
        get => _team;
        set
        {
            _team = value;
            UpdateHairAndBodyColors();
            UpdateSkinColor();
        }
    }

    public LemmingState(Lemming lemming, Team team)
    {
        _lemming = lemming;
        _team = team;
    }

    private void UpdateHairAndBodyColors()
    {
        if (HasPermanentSkill)
        {
            HairColor = _team.PermanentSkillHairColor;
            BodyColor = IsNeutral
                ? _team.NeutralBodyColor
                : _team.PermanentSkillBodyColor;
        }
        else
        {
            HairColor = _team.HairColor;
            BodyColor = IsNeutral
                ? _team.NeutralBodyColor
                : _team.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        SkinColor = IsZombie
            ? _team.ZombieSkinColor
            : _team.SkinColor;

        if (IsAcidLemming)
        {
            FootColor = _team.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            FootColor = _team.WaterLemmingFootColor;
        }
        else
        {
            FootColor = SkinColor;
        }
    }

    public void SetRawDataFromOther(LemmingState otherLemmingState)
    {
        _team = otherLemmingState._team;
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
}