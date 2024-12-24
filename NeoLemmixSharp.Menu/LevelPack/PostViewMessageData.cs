namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class PostViewMessageData
{
    public static List<PostViewMessageData> DefaultMessages { get; } = GenerateDefaultMessages();

    private static List<PostViewMessageData> GenerateDefaultMessages()
    {
        List<PostViewMessageData> result =
        [
            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 0,
                Lines = ["ROCK BOTTOM! I hope for your sake", "that you nuked that level."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 1,
                Lines = ["Better rethink your strategy before", "you try this level again!"]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 50,
                Lines = ["A little more practice on the level", "is definitely recommended."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 5,
                Lines = ["You got pretty close that time.", "Now try again for a few lemmings extra."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Below,
                NumericalValue = 1,
                Lines = ["OH NO, So near and yet so far...", "Maybe this time....."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 0,
                Lines = ["RIGHT ON. You can't get much closer", "than that. Let's try the next..."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Absolute,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 1,
                Lines = ["That level seemed no problem to you", "on that attempt. Onto the next...."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Above,
                NumericalValue = 20,
                Lines = ["You totally stormed that level!", "Let's see if you can do it again..."]
            },

            new()
            {
                AbsoluteOrPercentage = ResultType.Percentage,
                AboveOrBelow = ParityType.Equal,
                NumericalValue = 100,
                Lines = ["Superb! You rescued every lemming", "on that one. Can you do it again?"]
            },
        ];

        return result;
    }

    public required ResultType AbsoluteOrPercentage { get; init; }
    public required ParityType AboveOrBelow { get; init; }
    public required int NumericalValue { get; init; }
    public required List<string> Lines { get; init; }


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