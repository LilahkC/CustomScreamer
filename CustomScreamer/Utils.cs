using System.Diagnostics;

namespace CustomScreamer;

public class Utils
{
    private Stopwatch stopWatch = new();
    
    public bool Random(float chance, float time)
    {
        if (!stopWatch.IsRunning)
            stopWatch = Stopwatch.StartNew();

        if (!(stopWatch.Elapsed.Seconds >= time))
            return false;

        stopWatch.Restart();
        return System.Random.Shared.NextSingle() < chance / 100;
    }
}
