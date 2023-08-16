using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Engine.Teams;

namespace NeoLemmixSharp.Engine.Engine.Lemmings;

public sealed class LemmingState
{
    private Team _team;

    private bool _isClimber;
    private bool _isFloater;
    private bool _isGlider;
    private bool _isSlider;
    private bool _isSwimmer;
    private bool _isDisarmer;

    private bool _isNeutral;
    private bool _isZombie;

    public bool HasPermanentSkill { get; private set; }

    public bool IsClimber
    {
        get => _isClimber;
        set
        {
            _isClimber = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => _isFloater;
        set
        {
            _isFloater = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => _isGlider;
        set
        {
            _isGlider = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => _isSlider;
        set
        {
            _isSlider = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => _isSwimmer;
        set
        {
            _isSwimmer = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => _isDisarmer;
        set
        {
            _isDisarmer = value;
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => _isZombie;
        set
        {
            _isZombie = value;
            UpdateSkinColor();
        }
    }

    public bool IsNeutral
    {
        get => _isNeutral;
        set
        {
            _isNeutral = value;
            UpdateHairAndBodyColors();
        }
    }

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color BodyColor { get; private set; }

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

    private void UpdateHairAndBodyColors()
    {
        HasPermanentSkill = _isClimber ||
                            _isFloater ||
                            _isGlider ||
                            _isSlider ||
                            _isSwimmer ||
                            _isDisarmer;

        if (HasPermanentSkill)
        {
            HairColor = _team.BodyColor;
            BodyColor = _isNeutral
                ? _team.NeutralBodyColor
                : _team.HairColor;
        }
        else
        {
            HairColor = _team.HairColor;
            BodyColor = _isNeutral
                ? _team.NeutralBodyColor
                : _team.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        SkinColor = _isZombie
            ? _team.ZombieSkinColor
            : _team.SkinColor;
    }
}