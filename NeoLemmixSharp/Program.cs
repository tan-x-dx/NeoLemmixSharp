using System;

namespace NeoLemmixSharp;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        using var game = new NeoLemmixGame();
        game.Run();
    }
}