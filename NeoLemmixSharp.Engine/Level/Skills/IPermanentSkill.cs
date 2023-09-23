using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface IPermanentSkill
{
    void SetPermanentSkill(Lemming lemming, bool status);
    void TogglePermanentSkill(Lemming lemming);
}