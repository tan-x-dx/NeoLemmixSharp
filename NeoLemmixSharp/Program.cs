using System;

namespace NeoLemmixSharp;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        using var game = new NeoLemmixGame();
        game.Run();
    }
}