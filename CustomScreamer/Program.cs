using CustomScreamer.Renderer;
using SDL3;

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
            SDL.Delay(33);
        }

        Quit();
    }

    private static void Quit()
    {
        Game.Quit();
        Window.Destroy();
    }
}