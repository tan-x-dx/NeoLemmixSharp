using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Numerics;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    private readonly Lemming _lemming;
    private readonly PointerWrapper _tribeId;
    private readonly PointerWrapper _states;

    public LemmingColors LemmingColors;

    public bool HasPermanentSkill => (_states.UintValue & LemmingAbilityConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states.UintValue & LemmingAbilityConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states.UintValue & LemmingAbilityConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_states.UintValue & LemmingAbilityConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states.UintValue & LemmingAbilityConstants.AssignableSkillBitMask) == (1U << LemmingAbilityConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ClimberBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.FloaterBitIndex;
                states &= ~(1U << LemmingAbilityConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.GliderBitIndex;
                states &= ~(1U << LemmingAbilityConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.SliderBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.SwimmerBitIndex;
                states &= ~((1U << LemmingAbilityConstants.AcidLemmingBitIndex) | (1U << LemmingAbilityConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.DisarmerBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.PermanentFastForwardBitIndex);
            }
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(_lemming);
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.AcidLemmingBitIndex;
                states &= ~((1U << LemmingAbilityConstants.SwimmerBitIndex) | (1U << LemmingAbilityConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.WaterLemmingBitIndex;
                states &= ~((1U << LemmingAbilityConstants.SwimmerBitIndex) | (1U << LemmingAbilityConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ActiveBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsNeutral
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.NeutralBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => ((_states.UintValue >>> LemmingAbilityConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingAbilityConstants.ZombieBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingAbilityConstants.ZombieBitIndex);
            }
            LevelScreen.LemmingManager.UpdateZombieState(_lemming);
            UpdateSkinColor();
        }
    }

    public int TribeId => _tribeId.IntValue;

    public LemmingState(Lemming lemming, PointerWrapper tribeIdRef, PointerWrapper statesRef)
    {
        _lemming = lemming;
        _tribeId = tribeIdRef;
        _states = statesRef;
    }

    public void SetTribeAffiliation(int tribeId)
    {
        _tribeId.IntValue = tribeId;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
        UpdatePaintColor();
    }

    public void ClearAllPermanentSkills()
    {
        _states.UintValue &= ~LemmingAbilityConstants.PermanentSkillBitMask;
        UpdateHairAndBodyColors();
    }

    public void UpdateHairAndBodyColors()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);

        if (HasPermanentSkill)
        {
            LemmingColors.HairColor = tribe.ColorData.PermanentSkillHairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.PermanentSkillBodyColor;
        }
        else
        {
            LemmingColors.HairColor = tribe.ColorData.HairColor;
            LemmingColors.BodyColor = IsNeutral
                ? tribe.ColorData.NeutralBodyColor
                : tribe.ColorData.BodyColor;
        }
    }

    public void UpdateSkinColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);

        LemmingColors.SkinColor = IsZombie
            ? tribe.ColorData.ZombieSkinColor
            : tribe.ColorData.SkinColor;

        if (IsAcidLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            LemmingColors.FootColor = tribe.ColorData.WaterLemmingFootColor;
        }
        else
        {
            LemmingColors.FootColor = LemmingColors.SkinColor;
        }
    }

    public void UpdatePaintColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);
        LemmingColors.PaintColor = tribe.ColorData.PaintColor;
    }
}
