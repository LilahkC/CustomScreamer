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
        
        Window.SetShowWindow(true);
        
        while (Window.Loop)
        {
            Game.Update();
            
            Window.PoolEvents();

            Window.ClearRenderer();
            Window.RenderPresent();
        }

        Quit();
    }

    private static void Quit()
    {
        Game.Quit();
        Window.Quit();
    }
}