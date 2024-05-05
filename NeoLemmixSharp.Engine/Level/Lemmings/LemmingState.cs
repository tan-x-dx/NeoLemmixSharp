using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    public const int ClimberBitIndex = 0;
    public const int FloaterBitIndex = 1;
    public const int GliderBitIndex = 2;
    public const int SliderBitIndex = 3;
    public const int SwimmerBitIndex = 4;
    public const int DisarmerBitIndex = 5;

    private const uint PermanentSkillBitMask = (1U << ClimberBitIndex) |
                                               (1U << FloaterBitIndex) |
                                               (1U << GliderBitIndex) |
                                               (1U << SliderBitIndex) |
                                               (1U << SwimmerBitIndex) |
                                               (1U << DisarmerBitIndex);

    private const int PermanentFastForwardBitIndex = 20;

    public const int ActiveBitIndex = 29;
    public const int NeutralBitIndex = 30;
    public const int ZombieBitIndex = 31;

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
    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
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
                _states &= ~(1U << GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                _states &= ~(1U << FloaterBitIndex);
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
                _states &= ~(1U << FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                _states &= ~(1U << GliderBitIndex);
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

    public bool IsPermanentFastForwards
    {
        get => ((_states >> PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << PermanentFastForwardBitIndex;
            }
            else
            {
                _states &= ~(1U << PermanentFastForwardBitIndex);
            }
        }
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
                LevelScreen.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                _states &= ~(1U << ZombieBitIndex);
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

    private void SetBitToValue(uint bitMask, bool value)
    {
        if (value)
        {
            _states |= bitMask;
        }
        else
        {
            _states &= ~bitMask;
        }
        UpdateHairAndBodyColors();
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