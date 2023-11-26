using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    private const int ClimberBitIndex = 0;
    private const int FloaterBitIndex = 1;
    private const int GliderBitIndex = 2;
    private const int SliderBitIndex = 3;
    private const int SwimmerBitIndex = 4;
    private const int DisarmerBitIndex = 5;

    private const uint PermanentSkillBitMask = (1U << ClimberBitIndex) |
                                               (1U << FloaterBitIndex) |
                                               (1U << GliderBitIndex) |
                                               (1U << SliderBitIndex) |
                                               (1U << SwimmerBitIndex) |
                                               (1U << DisarmerBitIndex);

    private const int ActiveBitIndex = 29;
    private const int NeutralBitIndex = 30;
    private const int ZombieBitIndex = 31;

    private const uint AssignableSkillBitMask = (1U << ActiveBitIndex) |
                                                (1U << NeutralBitIndex) |
                                                (1U << ZombieBitIndex);

    private readonly Lemming _lemming;
    private Team _team;

    private uint _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color BodyColor { get; private set; }

    public bool HasPermanentSkill => (_states & PermanentSkillBitMask) != 0U;
    public bool CanHaveSkillsAssigned => (_states & AssignableSkillBitMask) == (1U << ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states >> ClimberBitIndex) & 1U) != 0U;
        set => SetBitToValue(1U << ClimberBitIndex, value);
    }

    public bool IsFloater
    {
        get => ((_states >> FloaterBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << FloaterBitIndex;
                _states &= 1U << GliderBitIndex; // Deliberately knock out the glider
            }
            else
            {
                _states &= 1U << FloaterBitIndex;
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states >> GliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << GliderBitIndex;
                _states &= 1U << FloaterBitIndex; // Deliberately knock out the floater
            }
            else
            {
                _states &= 1U << GliderBitIndex;
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states >> SliderBitIndex) & 1U) != 0U;
        set => SetBitToValue(1U << SliderBitIndex, value);
    }

    public bool IsSwimmer
    {
        get => ((_states >> SwimmerBitIndex) & 1U) != 0U;
        set => SetBitToValue(1U << SwimmerBitIndex, value);
    }

    public bool IsDisarmer
    {
        get => ((_states >> DisarmerBitIndex) & 1U) != 0U;
        set => SetBitToValue(1U << DisarmerBitIndex, value);
    }

    public bool IsNeutral
    {
        get => ((_states >> NeutralBitIndex) & 1U) != 0U;
        set => SetBitToValue(1U << NeutralBitIndex, value);
    }

    public bool IsActive
    {
        get => ((_states >> ActiveBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << ActiveBitIndex;
            }
            else
            {
                _states &= ~(1U << ActiveBitIndex);
            }
        }
    }

    public bool IsZombie
    {
        get => ((_states >> ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            if (value)
            {
                _states |= 1U << ZombieBitIndex;
                LevelConstants.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                _states &= ~(1U << ZombieBitIndex);
                LevelConstants.LemmingManager.DeregisterZombie(_lemming);
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
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    private void SetBitToValue(uint bitMask, bool value)
    {
        if (value)
        {
            _states |= bitMask;
        }
        else
        {
            _states &= bitMask;
        }
        UpdateHairAndBodyColors();
    }

    private void UpdateHairAndBodyColors()
    {
        if (HasPermanentSkill)
        {
            HairColor = _team.PermanentSkillBodyColor;
            BodyColor = IsNeutral
                ? _team.NeutralBodyColor
                : _team.PermanentSkillHairColor;
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
    }

    public void SetRawData(LemmingState otherLemmingState)
    {
        _team = otherLemmingState._team;
        _states = otherLemmingState._states;
    }

    public void SetRawData(Team team, uint rawData)
    {
        _team = team;
        _states = rawData;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }
}