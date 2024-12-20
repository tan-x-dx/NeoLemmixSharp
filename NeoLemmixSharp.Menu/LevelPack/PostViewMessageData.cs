namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class PostViewMessageData
{
    private static readonly PostViewMessageData[] _defaultMessages = GenerateDefaultMessages();
    public static ReadOnlySpan<PostViewMessageData> DefaultMessages => _defaultMessages;

    private static PostViewMessageData[] GenerateDefaultMessages()
    {
        var result = new PostViewMessageData[]
        {
            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 0,
                Line1 = "ROCK BOTTOM! I hope for your sake",
                Line2 = "that you nuked that level."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 1,
                Line1 = "Better rethink your strategy before",
                Line2 = "you try this level again!"
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 50,
                Line1 = "A little more practice on the level",
                Line2 = "is definitely recommended."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 5,
                Line1 = "You got pretty close that time.",
                Line2 = "Now try again for a few lemmings extra."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 1,
                Line1 = "OH NO, So near and yet so far...",
                Line2 = "Maybe this time....."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 0,
                Line1 = "RIGHT ON. You can't get much closer",
                Line2 = "than that. Let's try the next..."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 1,
                Line1 = "That level seemed no problem to you",
                Line2 = "on that attempt. Onto the next...."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 20,
                Line1 = "You totally stormed that level!",
                Line2 = "Let's see if you can do it again..."
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 100,
                Line1 = "Superb! You rescued every lemming",
                Line2 = "on that one. Can you do it again?"
            },
        };

        return result;
    }

    public required ResultType AbsoluteOrPercentage { get; init; }
    public required ParityType AboveOrBelow { get; init; }
    public required int NumericalValue { get; init; }
    public required string Line1 { get; init; }
    public required string Line2 { get; init; }

    public enum ResultType
    {
        Absolute,
        Percentage
    }

    public enum ParityType
    {
        Below,
        Equal,
        Above
    }
}