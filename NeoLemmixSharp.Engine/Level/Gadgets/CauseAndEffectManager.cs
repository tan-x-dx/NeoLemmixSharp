using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class CauseAndEffectManager :
    IPerfectHasher<GadgetTrigger>,
    IPerfectHasher<GadgetBehaviour>
{
    private readonly GadgetTrigger[] _allTriggers;
    private readonly GadgetBehaviour[] _allBehaviours;

    private readonly List<CauseAndEffectData> _causeAndEffectData = new(256);

    public CauseAndEffectManager(
        GadgetTrigger[] allTriggers,
        GadgetBehaviour[] allBehaviours)
    {
        _allTriggers = allTriggers;
        this.AssertUniqueIds(new ReadOnlySpan<GadgetTrigger>(_allTriggers));
        Array.Sort(_allTriggers, this);

        _allBehaviours = allBehaviours;
        this.AssertUniqueIds(new ReadOnlySpan<GadgetBehaviour>(_allBehaviours));
        Array.Sort(_allBehaviours, this);
    }

    public void RegisterCauseAndEffectData(CauseAndEffectData causeAndEffectLemmingData)
    {
        _causeAndEffectData.Add(causeAndEffectLemmingData);
    }

    public void ResetBehaviours()
    {
        _causeAndEffectData.Clear();

        foreach (var behaviour in _allBehaviours)
        {
            behaviour.Reset();
        }
    }

    public void Tick()
    {
        foreach (var causeAndEffectDatum in _causeAndEffectData)
        {
            var behaviour = _allBehaviours[causeAndEffectDatum.GadgetBehaviourId];

            behaviour.PerformBehaviour(causeAndEffectDatum.LemmingId);
        }
    }

    int IPerfectHasher<GadgetTrigger>.NumberOfItems => _allTriggers.Length;
    int IPerfectHasher<GadgetTrigger>.Hash(GadgetTrigger item) => item.Id;
    GadgetTrigger IPerfectHasher<GadgetTrigger>.UnHash(int index) => _allTriggers[index];

    int IPerfectHasher<GadgetBehaviour>.NumberOfItems => _allBehaviours.Length;
    int IPerfectHasher<GadgetBehaviour>.Hash(GadgetBehaviour item) => item.Id;
    GadgetBehaviour IPerfectHasher<GadgetBehaviour>.UnHash(int index) => _allBehaviours[index];
}
