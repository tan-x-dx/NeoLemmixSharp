using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Teams;

namespace NeoLemmixSharp.Engine.Engine.Lemmings;

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

    private const int NeutralBitIndex = 30;
    private const int ZombieBitIndex = 31;

    private const uint AssignableSkillBitMask = (1U << NeutralBitIndex) |
                                                (1U << ZombieBitIndex);

    private readonly SmallBitArray _states = new();

    private Team _team;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color BodyColor { get; private set; }

    public bool HasPermanentSkill => _states.GetBitMask(PermanentSkillBitMask) != 0U;
    public bool CanHaveSkillsAssigned => _states.GetBitMask(AssignableSkillBitMask) == 0U;

    public bool IsClimber
    {
        get => _states.GetBit(ClimberBitIndex);
        set => SetBitToValue(ClimberBitIndex, value);
    }

    public bool IsFloater
    {
        get => _states.GetBit(FloaterBitIndex);
        set => SetBitToValue(FloaterBitIndex, value);
    }

    public bool IsGlider
    {
        get => _states.GetBit(GliderBitIndex);
        set => SetBitToValue(GliderBitIndex, value);
    }

    public bool IsSlider
    {
        get => _states.GetBit(SliderBitIndex);
        set => SetBitToValue(SliderBitIndex, value);
    }

    public bool IsSwimmer
    {
        get => _states.GetBit(SwimmerBitIndex);
        set => SetBitToValue(SwimmerBitIndex, value);
    }

    public bool IsDisarmer
    {
        get => _states.GetBit(DisarmerBitIndex);
        set => SetBitToValue(DisarmerBitIndex, value);
    }

    public bool IsNeutral
    {
        get => _states.GetBit(NeutralBitIndex);
        set => SetBitToValue(NeutralBitIndex, value);
    }

    public bool IsZombie
    {
        get => _states.GetBit(ZombieBitIndex);
        set
        {
            if (value)
            {
                _states.SetBit(ZombieBitIndex);
            }
            else
            {
                _states.ClearBit(ZombieBitIndex);
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

    public LemmingState(Team team)
    {
        _team = team;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    private void SetBitToValue(int bitIndex, bool value)
    {
        if (value)
        {
            _states.SetBit(bitIndex);
        }
        else
        {
            _states.ClearBit(bitIndex);
        }
        UpdateHairAndBodyColors();
    }

    private void UpdateHairAndBodyColors()
    {
        if (HasPermanentSkill)
        {
            HairColor = _team.BodyColor;
            BodyColor = IsNeutral
                ? _team.NeutralBodyColor
                : _team.HairColor;
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
}