using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Tribes;
using System.Numerics;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingState : ISnapshotDataConvertible<LemmingStateSnapshotData>
{
    private readonly Lemming _lemming;
    private int _tribeId;

    private uint _states;

    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color FootColor { get; private set; }
    public Color BodyColor { get; private set; }
    public Color PaintColor { get; private set; }

    public bool HasPermanentSkill => (_states & LemmingStateConstants.PermanentSkillBitMask) != 0U;
    public bool HasLiquidAffinity => (_states & LemmingStateConstants.LiquidAffinityBitMask) != 0U;
    public bool HasSpecialFallingBehaviour => (_states & LemmingStateConstants.SpecialFallingBehaviourBitMask) != 0U;
    public int NumberOfPermanentSkills => BitOperations.PopCount(_states & LemmingStateConstants.PermanentSkillBitMask);

    /// <summary>
    /// Must be active and NOT zombie and NOT neutral
    /// </summary>
    public bool CanHaveSkillsAssigned => (_states & LemmingStateConstants.AssignableSkillBitMask) == (1U << LemmingStateConstants.ActiveBitIndex);

    public bool IsClimber
    {
        get => ((_states >>> LemmingStateConstants.ClimberBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.ClimberBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.ClimberBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsFloater
    {
        get => ((_states >>> LemmingStateConstants.FloaterBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.FloaterBitIndex;
                _states &= ~(1U << LemmingStateConstants.GliderBitIndex); // Deliberately knock out the glider
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.FloaterBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsGlider
    {
        get => ((_states >>> LemmingStateConstants.GliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.GliderBitIndex;
                _states &= ~(1U << LemmingStateConstants.FloaterBitIndex); // Deliberately knock out the floater
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.GliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSlider
    {
        get => ((_states >>> LemmingStateConstants.SliderBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.SliderBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.SliderBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsSwimmer
    {
        get => ((_states >>> LemmingStateConstants.SwimmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.SwimmerBitIndex;
                _states &= ~((1U << LemmingStateConstants.AcidLemmingBitIndex) | (1U << LemmingStateConstants.WaterLemmingBitIndex)); // Deliberately knock out the acid/water lemmings
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.SwimmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsDisarmer
    {
        get => ((_states >>> LemmingStateConstants.DisarmerBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.DisarmerBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.DisarmerBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsPermanentFastForwards
    {
        get => ((_states >>> LemmingStateConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.PermanentFastForwardBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.PermanentFastForwardBitIndex);
            }

            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(_lemming);
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states >>> LemmingStateConstants.AcidLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.AcidLemmingBitIndex;
                _states &= ~((1U << LemmingStateConstants.SwimmerBitIndex) | (1U << LemmingStateConstants.WaterLemmingBitIndex)); // Deliberately knock out the swimmer/water lemmings
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.AcidLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsWaterLemming
    {
        get => ((_states >>> LemmingStateConstants.WaterLemmingBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.WaterLemmingBitIndex;
                _states &= ~((1U << LemmingStateConstants.SwimmerBitIndex) | (1U << LemmingStateConstants.AcidLemmingBitIndex)); // Deliberately knock out the swimmer/acid lemmings
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.WaterLemmingBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsActive
    {
        get => ((_states >>> LemmingStateConstants.ActiveBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.ActiveBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.ActiveBitIndex);
            }
        }
    }

    public bool IsNeutral
    {
        get => ((_states >>> LemmingStateConstants.NeutralBitIndex) & 1U) != 0U;
        set
        {
            if (value)
            {
                _states |= 1U << LemmingStateConstants.NeutralBitIndex;
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.NeutralBitIndex);
            }
            UpdateHairAndBodyColors();
        }
    }

    public bool IsZombie
    {
        get => ((_states >>> LemmingStateConstants.ZombieBitIndex) & 1U) != 0U;
        set
        {
            if (IsZombie == value)
                return;

            if (value)
            {
                _states |= 1U << LemmingStateConstants.ZombieBitIndex;
                LevelScreen.LemmingManager.RegisterZombie(_lemming);
            }
            else
            {
                _states &= ~(1U << LemmingStateConstants.ZombieBitIndex);
                LevelScreen.LemmingManager.DeregisterZombie(_lemming);
            }
            UpdateSkinColor();
        }
    }

    public Tribe TribeAffiliation
    {
        get => LevelScreen.TribeManager.AllItems[_tribeId];
        set
        {
            _tribeId = value.Id;
            UpdateHairAndBodyColors();
            UpdateSkinColor();
            PaintColor = LevelScreen.TribeManager.AllItems[_tribeId].ColorData.PaintColor;
        }
    }

    public LemmingState(Lemming lemming, int tribeId)
    {
        _lemming = lemming;
        _tribeId = tribeId;
    }

    public void ClearAllPermanentSkills()
    {
        _states &= ~LemmingStateConstants.PermanentSkillBitMask;
        UpdateHairAndBodyColors();
    }

    private void UpdateHairAndBodyColors()
    {
        ref readonly var tribeColorData = ref LevelScreen.TribeManager.AllItems[_tribeId].ColorData;

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

    private void UpdateSkinColor()
    {
        ref readonly var tribeColorData = ref LevelScreen.TribeManager.AllItems[_tribeId].ColorData;

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

    public void SetRawDataFromOther(LemmingState otherLemmingState)
    {
        _tribeId = otherLemmingState._tribeId;
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
        _tribeId = lemmingStateSnapshotData.TribeId;
        _states = lemmingStateSnapshotData.StateData;
        UpdateHairAndBodyColors();
        UpdateSkinColor();
    }

    public void WriteToSnapshotData(out LemmingStateSnapshotData data)
    {
        data = new LemmingStateSnapshotData(_tribeId, _states);
    }
}
