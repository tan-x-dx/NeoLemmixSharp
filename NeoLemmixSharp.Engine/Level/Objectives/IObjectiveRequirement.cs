namespace NeoLemmixSharp.Engine.Level.Objectives;

public interface IObjectiveRequirement
{
    bool IsSatisfied { get; }
}