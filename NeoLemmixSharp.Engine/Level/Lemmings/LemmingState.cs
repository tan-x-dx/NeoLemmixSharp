using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Numerics;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    private readonly Lemming _lemming;
    private readonly PointerWrapper _tribeId;
    private readonly PointerWrapper _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color BodyColor { get; private set; }
    public Color FootColor { get; private set; }
    public Color PaintColor { get; private set; }

    public bool HasPermanentSkill => (_states.UintValue & LemmingStateConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states.UintValue & LemmingStateConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states.UintValue & LemmingStateConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_states.UintValue & LemmingStateConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states.UintValue & LemmingStateConstants.AssignableSkillBitMask) == (1U << LemmingStateConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states.UintValue >>> LemmingStateConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.ClimberBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_states.UintValue >>> LemmingStateConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.FloaterBitIndex;
                states &= ~(1U << LemmingStateConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states.UintValue >>> LemmingStateConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.GliderBitIndex;
                states &= ~(1U << LemmingStateConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states.UintValue >>> LemmingStateConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.SliderBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_states.UintValue >>> LemmingStateConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.SwimmerBitIndex;
                states &= ~((1U << LemmingStateConstants.AcidLemmingBitIndex) | (1U << LemmingStateConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_states.UintValue >>> LemmingStateConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.DisarmerBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_states.UintValue >>> LemmingStateConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.PermanentFastForwardBitIndex);
            }
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(_lemming);
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states.UintValue >>> LemmingStateConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.AcidLemmingBitIndex;
                states &= ~((1U << LemmingStateConstants.SwimmerBitIndex) | (1U << LemmingStateConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_states.UintValue >>> LemmingStateConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.WaterLemmingBitIndex;
                states &= ~((1U << LemmingStateConstants.SwimmerBitIndex) | (1U << LemmingStateConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_states.UintValue >>> LemmingStateConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.ActiveBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsNeutral
    {
        get => ((_states.UintValue >>> LemmingStateConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.NeutralBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => ((_states.UintValue >>> LemmingStateConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            ref var states = ref _states.UintValue;
            if (value)
            {
                states |= 1U << LemmingStateConstants.ZombieBitIndex;
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.ZombieBitIndex);
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
        _states.UintValue &= ~LemmingStateConstants.PermanentSkillBitMask;
        UpdateHairAndBodyColors();
    }

    public void UpdateHairAndBodyColors()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);
        ref readonly var tribeColorData = ref tribe.ColorData;

        if (HasPermanentSkill)
        {
            HairColor = tribeColorData.PermanentSkillHairColor;
            BodyColor = IsNeutral
                ? tribeColorData.NeutralBodyColor
                : tribeColorData.PermanentSkillBodyColor;
        }
        else
        {
            HairColor = tribeColorData.HairColor;
            BodyColor = IsNeutral
                ? tribeColorData.NeutralBodyColor
                : tribeColorData.BodyColor;
        }
    }

    public void UpdateSkinColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);
        ref readonly var tribeColorData = ref tribe.ColorData;

        SkinColor = IsZombie
            ? tribeColorData.ZombieSkinColor
            : tribeColorData.SkinColor;

        if (IsAcidLemming)
        {
            FootColor = tribeColorData.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            FootColor = tribeColorData.WaterLemmingFootColor;
        }
        else
        {
            FootColor = SkinColor;
        }
    }

    public void UpdatePaintColor()
    {
        var tribe = LevelScreen.TribeManager.GetTribe(_tribeId.IntValue);
        PaintColor = tribe.ColorData.PaintColor;
    }
}
