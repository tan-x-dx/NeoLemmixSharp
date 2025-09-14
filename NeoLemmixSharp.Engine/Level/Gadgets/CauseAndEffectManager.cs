using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class CauseAndEffectManager :
    IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>,
    IPerfectHasher<GadgetBehaviour>
{
    private readonly GadgetTrigger[] _allTriggers;
    private readonly GadgetBehaviour[] _allBehaviours;
    private readonly BitArraySet<CauseAndEffectManager, ArrayBitBuffer, GadgetTrigger> _indeterminateTriggers;
    private readonly SimpleList<CauseAndEffectData> _causeAndEffectData = new(256);

    public ReadOnlySpan<GadgetBehaviour> AllBehaviours => new(_allBehaviours);

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

        _indeterminateTriggers = new BitArraySet<CauseAndEffectManager, ArrayBitBuffer, GadgetTrigger>(this);
    }

    public void RegisterCauseAndEffectData(CauseAndEffectData causeAndEffectLemmingData)
    {
        _causeAndEffectData.Add(causeAndEffectLemmingData);
    }

    public void ResetBehaviours()
    {
        _causeAndEffectData.Clear();
        _indeterminateTriggers.Fill();

        foreach (var trigger in _allTriggers)
        {
            trigger.Reset();
        }

        foreach (var behaviour in _allBehaviours)
        {
            behaviour.Reset();
        }
    }

    public void MarkTriggerAsEvaluated(GadgetTrigger gadgetTrigger)
    {
        _indeterminateTriggers.Remove(gadgetTrigger);
    }

    public void FlagIndeterminateTriggersAsNotTriggered()
    {
        foreach (var trigger in _indeterminateTriggers)
        {
            trigger.DetermineTrigger(false);
        }
    }

    public void Tick()
    {
        for (var i = 0; i < _causeAndEffectData.Count; i++)
        {
            var causeAndEffectDatum = _causeAndEffectData[i];
            var behaviour = _allBehaviours[causeAndEffectDatum.GadgetBehaviourId];
            behaviour.PerformBehaviour(causeAndEffectDatum.TriggerData);
        }
    }

    int IPerfectHasher<GadgetTrigger>.NumberOfItems => _allTriggers.Length;
    int IPerfectHasher<GadgetTrigger>.Hash(GadgetTrigger item) => item.Id;
    GadgetTrigger IPerfectHasher<GadgetTrigger>.UnHash(int index) => _allTriggers[index];
    void IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_allTriggers.Length);

    int IPerfectHasher<GadgetBehaviour>.NumberOfItems => _allBehaviours.Length;
    int IPerfectHasher<GadgetBehaviour>.Hash(GadgetBehaviour item) => item.Id;
    GadgetBehaviour IPerfectHasher<GadgetBehaviour>.UnHash(int index) => _allBehaviours[index];
}
