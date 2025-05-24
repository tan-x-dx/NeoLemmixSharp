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
        get => ((_states >>> EngineConstants.ClimberBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.FloaterBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.GliderBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.SliderBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.SwimmerBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.DisarmerBitIndex) & 1U) != 0U;
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

    public bool IsPermanentFastForwards
    {
        get => ((_states >>> EngineConstants.PermanentFastForwardBitIndex) & 1U) != 0U;
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

            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(_lemming);
        }
    }

    public bool IsAcidLemming
    {
        get => ((_states >>> EngineConstants.AcidLemmingBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.WaterLemmingBitIndex) & 1U) != 0U;
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
        get => ((_states >>> EngineConstants.ActiveBitIndex) & 1U) != 0U;
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

    public bool IsNeutral
    {
        get => ((_states >>> EngineConstants.NeutralBitIndex) & 1U) != 0U;
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

    public bool IsZombie
    {
        get => ((_states >>> EngineConstants.ZombieBitIndex) & 1U) != 0U;
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

    private void UpdateHairAndBodyColors()
    {
        var tribe = LevelScreen.TribeManager.AllItems[_tribeId].ColorData;

        if (HasPermanentSkill)
        {
            HairColor = tribe.PermanentSkillHairColor;
            BodyColor = IsNeutral
                ? tribe.NeutralBodyColor
                : tribe.PermanentSkillBodyColor;
        }
        else
        {
            HairColor = tribe.HairColor;
            BodyColor = IsNeutral
                ? tribe.NeutralBodyColor
                : tribe.BodyColor;
        }
    }

    private void UpdateSkinColor()
    {
        var tribe = LevelScreen.TribeManager.AllItems[_tribeId].ColorData;

        var skinColor = IsZombie
            ? tribe.ZombieSkinColor
            : tribe.SkinColor;

        SkinColor = skinColor;

        if (IsAcidLemming)
        {
            FootColor = tribe.AcidLemmingFootColor;
        }
        else if (IsWaterLemming)
        {
            FootColor = tribe.WaterLemmingFootColor;
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