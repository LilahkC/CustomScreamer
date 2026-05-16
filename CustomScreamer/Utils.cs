using System.Diagnostics;
using SDL3;

namespace CustomScreamer;

public class Utils
{
    private Stopwatch stopWatch = new();
    
    public bool Random(float chance, float time)
    {
        bool isCooking = true;
        int count = 0;
        while (isCooking)
        {
            if (System.Random.Shared.NextSingle() < chance / 100)
                isCooking = false;
            else
                count++;
        }

        SDL.Delay((uint)(count * time) * 1000);
        return true;
    }
}
