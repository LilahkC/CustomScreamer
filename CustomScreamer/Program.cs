using CustomScreamer.Renderer;

namespace CustomScreamer;

internal static class Program
{
    private static readonly Window Window = new();
    private static readonly Game.Game Game = new();

    private static void Main()
    {
        Window.Initialize();
        Game.Initialize();
        
        while (Window.Loop)
        {
            Game.Update();
            Window.Update();
        }

        Quit();
    }

    private static void Quit()
    {
        Game.Quit();
        Window.Quit();
    }
}