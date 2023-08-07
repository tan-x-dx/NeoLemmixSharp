namespace NeoLemmixSharp.Engine.Engine.Gadgets.Inputs;

public sealed class Input : IEquatable<Input>
{
    public int Id { get; }
    public string Name { get; }
    public InputType Type { get; }

    public Input(int id, string name, InputType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public bool Equals(Input? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Input other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}

public enum InputType
{
    Continuous,
    RisingEdge
}