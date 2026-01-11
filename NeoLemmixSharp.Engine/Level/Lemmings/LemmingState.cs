using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Tribes;
using System.Numerics;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState
{
    private readonly Lemming _lemming;
    private readonly PointerWrapper<int> _tribeId;
    private readonly PointerWrapper<uint> _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color FootColor { get; private set; }
    public Color BodyColor { get; private set; }
    public Color PaintColor { get; private set; }

    public bool HasPermanentSkill => (_states.Value & LemmingStateConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states.Value & LemmingStateConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states.Value & LemmingStateConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_states.Value & LemmingStateConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states.Value & LemmingStateConstants.AssignableSkillBitMask) == (1U << LemmingStateConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states.Value >>> LemmingStateConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            ref var states = ref _states.Value;
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
        get => ((_states.Value >>> LemmingStateConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            ref var states = ref _states.Value;
            if (value)
            {
                states |= 1U << LemmingStateConstants.ZombieBitIndex;
                LevelScreen.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                states &= ~(1U << LemmingStateConstants.ZombieBitIndex);
                LevelScreen.LemmingManager.DeregisterZombie(_lemming);
            }
            UpdateSkinColor();
        }
    }

    public Tribe TribeAffiliation
    {
        get => LevelScreen.TribeManager.GetTribe(_tribeId.Value);
        set
        {
            _tribeId.Value = value.Id;
            UpdateHairAndBodyColors();
            UpdateSkinColor();
            PaintColor = LevelScreen.TribeManager.GetTribe(_tribeId.Value).ColorData.PaintColor;
        }
    }

    public LemmingState(Lemming lemming, PointerWrapper<int> tribeIdRef, PointerWrapper<uint> statesRef)
    {
        _tribeId = tribeIdRef;
        _states = statesRef;
        _lemming = lemming;
    }

    public void ClearAllPermanentSkills()
    {
        _states.Value &= ~LemmingStateConstants.PermanentSkillBitMask;
        UpdateHairAndBodyColors();
    }

    public void UpdateHairAndBodyColors()
    {
        ref readonly var tribeColorData = ref LevelScreen.TribeManager.GetTribe(_tribeId.Value).ColorData;

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
        ref readonly var tribeColorData = ref LevelScreen.TribeManager.GetTribe(_tribeId.Value).ColorData;

        var skinColor = IsZombie
            ? tribeColorData.ZombieSkinColor
            : tribeColorData.SkinColor;

        SkinColor = skinColor;

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
            FootColor = skinColor;
        }
    }

    public void SetData(int tribeId, uint rawData)
    {
        _tribeId.Value = tribeId;
        _states.Value = rawData;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }
}
