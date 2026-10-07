using CustomScreamer.Renderer;
using SDL3;
namespace CustomScreamer;

internal static class Program
{
    private static readonly Window Window = new();
    private const int MaxFps = 30;
    
    private static void Main()
    {
        const ulong FrameNs = 1_000_000_000UL / MaxFps;

        while (Window.Loop)
        {
            ulong frameStart = SDL.GetTicksNS();

            Window.Update();
            Game.Game.Update();

            ulong elapsed = SDL.GetTicksNS() - frameStart;
            if (elapsed < FrameNs)
                SDL.DelayNS(FrameNs - elapsed);
        }
        
        Quit();
    }
    
    private static void Quit()
    {
        Window.Destroy();
    }
}
